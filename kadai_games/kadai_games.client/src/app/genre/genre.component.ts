import { Component, OnInit } from '@angular/core';
import { MasterService } from '../services/master.service';
import { ActivatedRoute, Router } from '@angular/router';

interface Genre {
  genre_Id?: number;
  genre_Name: string;
  createdUser?: string;
  createDate?: string;
  delete_Flg?: boolean;
}

@Component({
  selector: 'app-genre',
  templateUrl: './genre.component.html',
  styleUrls: ['./genre.component.css']
})
export class GenreComponent implements OnInit {
  genreId!: number; // ジャンルID
  genreDetail: Genre = { genre_Name: '' }; // 詳細情報
  queryParams: any = {}; // クエリパラメータ
  serverError: string | null = null; // サーバーエラーメッセージ

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private masterService: MasterService
  ) { }

  ngOnInit(): void {
    // URLパラメータ取得
    this.route.params.subscribe((params) => {
      this.genreId = +params['id'];
      if (this.genreId) {
        this.loadGenreDetails();
      }
    });

    // クエリパラメータ取得
    this.route.queryParams.subscribe((queryParams) => {
      this.queryParams = { ...queryParams };
    });
  }
  
  //ジャンル詳細情報を取得 
  loadGenreDetails(): void {
    this.masterService.getGenreById(this.genreId).subscribe({
      next: (data) => (this.genreDetail = data),
      error: (err) => {
        console.error('エラーが発生しました:', err);
        this.serverError = 'ジャンルの詳細情報を取得できませんでした。';
      }
    });
  }
  
  //ジャンルを作成 
  createGenre(): void {
    this.serverError = null;
    const newGenre: Genre = {
      genre_Name: this.genreDetail.genre_Name.trim(),
      createdUser: 'admin',
      createDate: new Date().toISOString(),
      delete_Flg: false,
    };

    this.masterService.createGenre(newGenre).subscribe({
      next: () => this.router.navigate(['/Genres']),
      error: (errorResponse) => {
        console.error('作成エラー:', errorResponse);

        // errors?.Genre_Name がある場合
        if (errorResponse.error?.errors?.Genre_Name?.[0]) {
          this.serverError = errorResponse.error.errors.Genre_Name[0];
        }
        // message プロパティがある場合
        else if (errorResponse.error?.message) {
          this.serverError = errorResponse.error.message;
        } 
      }
    });
  }
  
  //ジャンルを更新
  updateGenre(): void {
    this.serverError = null; 
    const updatedGenre = {
      ...this.genreDetail,
      createDate: new Date().toISOString(),
      createdUser: 'admin',
    };

    this.masterService.updateGenre(this.genreId, updatedGenre).subscribe({
      next: () => {
        alert('ジャンルが更新されました');
        this.router.navigate(['/Genres'])
      },
      error: (errorResponse) => {
        this.serverError = errorResponse.error.message;
      }
    });
  }
  
  //ジャンルを削除
  deleteGenre(): void {
    if (confirm('このジャンルを削除してもよろしいですか？')) {
      this.masterService.deleteGenre(this.genreId).subscribe({
        next: () => {
          alert('ジャンルが削除されました');
          this.router.navigate(['/Genres'])},
        error: (errorResponse) => {
          this.serverError = errorResponse.error.message;
        }
      });
    }
  }

  //一覧画面に戻る 
  goBack(): void {
    this.router.navigate(['/Genres'], { queryParams: this.queryParams });
  }
}
