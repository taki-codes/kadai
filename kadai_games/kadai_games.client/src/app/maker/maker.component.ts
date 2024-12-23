import { Component, OnInit } from '@angular/core';
import { MasterService } from '../services/master.service';
import { ActivatedRoute, Router } from '@angular/router';

interface Maker {
  maker_Id?: number;
  maker_Name: string;
  maker_Address?: string;
  createdUser?: string;
  createDate?: string;
  delete_Flg?: boolean;
}

@Component({
  selector: 'app-maker',
  templateUrl: './maker.component.html',
  styleUrls: ['./maker.component.css']
})
export class MakerComponent implements OnInit {
  makerId!: number; // メーカーID
  makerDetail: Maker = { maker_Name: '' }; // 詳細情報
  queryParams: any = {}; // クエリパラメータ
  serverError: string | null = null; // サーバーエラーメッセージ
  serverErrors: string[] = [];   // サーバーエラーを格納する配列
  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private masterService: MasterService
  ) { }

  ngOnInit(): void {
    // URLパラメータ取得
    this.route.params.subscribe((params) => {
      this.makerId = +params['id'];
      if (this.makerId) {
        this.loadMakerDetails();
      }
    });

    // クエリパラメータ取得
    this.route.queryParams.subscribe((queryParams) => {
      this.queryParams = { ...queryParams };
    });
  }

  // メーカー詳細情報を取得
  loadMakerDetails(): void {
    this.masterService.getMakerById(this.makerId).subscribe({
      next: (data) => (this.makerDetail = data),
      error: (err) => {
        console.error('エラーが発生しました:', err);
        this.serverError = 'メーカーの詳細情報を取得できませんでした。';
      }
    });
  }

  // メーカーを作成
  createMaker(): void {
    this.serverError = null;
    const newMaker: Maker = {
      maker_Name: this.makerDetail.maker_Name.trim(),
      maker_Address: this.makerDetail.maker_Address?.trim() || '',
      createdUser: 'admin',
      createDate: new Date().toISOString(),
      delete_Flg: false,
    };

    this.masterService.createMaker(newMaker).subscribe({
      next: () => this.router.navigate(['/Makers']),
      error: (errorResponse) => {
        console.error('作成エラー:', errorResponse);

        // エラーメッセージをリセット
        this.serverErrors = [];

        // エラーメッセージを収集
        if (errorResponse.error?.errors?.Maker_Name) {
          this.serverErrors.push(...errorResponse.error.errors.Maker_Name);
        }
        if (errorResponse.error?.errors?.Maker_Address) {
          this.serverErrors.push(...errorResponse.error.errors.Maker_Address);
        }
        if (errorResponse.error?.message) {
          this.serverErrors.push(errorResponse.error.message);
        }
      }
    });
  }

  // メーカーを更新
  updateMaker(): void {
    this.serverError = null;
    const updatedMaker = {
      ...this.makerDetail,
      createDate: new Date().toISOString(),
      createdUser: 'admin',
    };

    this.masterService.updateMaker(this.makerId, updatedMaker).subscribe({
      next: () => {
        alert('メーカーが更新されました');
        this.router.navigate(['/Makers']);
      },
      error: (errorResponse) => {
        // エラーメッセージをリセット
        this.serverErrors = [];

        // エラーメッセージを収集
        if (errorResponse.error?.errors?.Maker_Name) {
          this.serverErrors.push(...errorResponse.error.errors.Maker_Name);
        }
        if (errorResponse.error?.errors?.Maker_Address) {
          this.serverErrors.push(...errorResponse.error.errors.Maker_Address);
        }
        if (errorResponse.error?.message) {
          this.serverErrors.push(...errorResponse.error.message);
        }
      }
    });
  }

  // メーカーを削除
  deleteMaker(): void {
    if (confirm('このメーカーを削除してもよろしいですか？')) {
      this.masterService.deleteMaker(this.makerId).subscribe({
        next: () => {
          alert('メーカーが削除されました');
          this.router.navigate(['/Makers']);
        },
        error: (errorResponse) => {
          // エラーメッセージをリセット
          this.serverErrors = [];

          // エラーメッセージを収集
          if (errorResponse.error?.errors?.Maker_Name) {
            this.serverErrors.push(...errorResponse.error.errors.Maker_Name);
          }
          if (errorResponse.error?.errors?.Maker_Address) {
            this.serverErrors.push(...errorResponse.error.errors.Maker_Address);
          }
          if (errorResponse.error?.message) {
            this.serverErrors.push(errorResponse.error.message);
          }
        }
      });
    }
  }

  // 一覧画面に戻る
  goBack(): void {
    this.router.navigate(['/Makers'], { queryParams: this.queryParams });
  }
}
