  import { Component, OnInit, Inject } from '@angular/core';
  import { ActivatedRoute, Router } from '@angular/router';
  import { GamesService } from '../services/games.service';
  import { MatDialog } from '@angular/material/dialog';
  import { MAT_DIALOG_DATA } from '@angular/material/dialog';

  interface Game {
    game_Id?: number;
    title: string;
    maker_Id: number;
    genre_Id: number;
    sales_Count: number;
    memo?: string;
    createdUser?: string;
    createDate?: string;
    updateUser?: string;
    updateDate?: string;
    delete_Flg?: boolean;
  }

  interface Maker {
    maker_Id: number;
    maker_Name: string;
  }

  interface Genre {
    genre_Id: number;
    genre_Name: string;
  }

  @Component({
    selector: 'app-game',
    templateUrl: './game.component.html',
    styleUrls: ['./game.component.css'],
  })
  export class GameComponent implements OnInit {
    game: Game = {
      game_Id: 0,
      title: '',
      maker_Id: 0,
      genre_Id: 0,
      sales_Count: 0,
      memo: '',
      createdUser: 'admin',
      createDate: new Date().toISOString(),
      updateUser: 'admin',
      updateDate: new Date().toISOString(),
      delete_Flg: false,
    };

    makers: Maker[] = [];
    genres: Genre[] = [];
    isNew = true;
    errorMessage: string | null = null;
    queryParams: any = {}; // 初期化
    // 他のプロパティ
    titleError: string | null = null;
    makerError: string | null = null;
    genreError: string | null = null;
    salesCountError: string | null = null;


    constructor(
      private route: ActivatedRoute,
      public router: Router,
      private gamesService: GamesService,
      private dialog: MatDialog
    ) { }

    ngOnInit(): void {
      this.route.queryParams.subscribe((params) => {
        this.queryParams = { ...params }; // クエリパラメータを保存
      });

      this.loadMakers();
      this.loadGenres();

      const id = this.route.snapshot.paramMap.get('id');
      if (id) {
        this.isNew = false;
        this.loadGame(+id);
      }
    }

    //メーカ名取得
    loadMakers(): void {
      this.gamesService.getMakers().subscribe({
        next: (data) => (this.makers = data),
        error: () => {
          this.errorMessage = 'メーカーの取得に失敗しました。';
        },
      });
    }

    //ジャンル名取得
    loadGenres(): void {
      this.gamesService.getGenres().subscribe({
        next: (data) => (this.genres = data),
        error: () => {
          this.errorMessage = 'ジャンルの取得に失敗しました。';
        },
      });
    }

    //詳細の取得
    loadGame(id: number): void {
      this.gamesService.getGameById(id).subscribe({
        next: (data) => {
          this.game = { ...data };
        },
        error: () => {
          this.errorMessage = 'ゲーム詳細の取得に失敗しました。';
        },
      });
    }

    //クエリ保存
    goBack(): void {
      this.router.navigate(['/games'], {
        queryParams: this.queryParams, // クエリパラメータを保持してリスト画面に戻る
      });
    }

    //保存
    saveGame(): void {
      // クライアント側でのフィールドバリデーション
      if (!this.validateFields()) {
        this.errorMessage = '入力内容を確認してください。';
        return;
      }

      if (this.isNew) {
        // 新規作成
        this.gamesService.createGame(this.game).subscribe({
          next: () => {
            this.router.navigate(['/games']);
          },
          error: (err) => {
            this.handleServerError(err);
          },
        });
      } else {
        // 更新
        this.gamesService.updateGame(this.game.game_Id!, this.game).subscribe({
          next: () => {
            this.router.navigate(['/games']);
          },
          error: (err) => {
            this.handleServerError(err);
          },
        });
      }
    }

    // サーバーエラーを処理するヘルパーメソッド
    handleServerError(err: any): void {
      const errors = err.error?.errors || {};

      // サーバーからのエラーを割り当て
      this.titleError = errors.Title ? errors.Title[0] : null;
      this.makerError = errors.Maker_Id ? errors.Maker_Id[0] : null;
      this.genreError = errors.Genre_Id ? errors.Genre_Id[0] : null;
      this.salesCountError = errors.Sales_Count ? errors.Sales_Count[0] : null;

      this.errorMessage = '入力内容を確認してください。';
    }
    // クライアント側のフィールドバリデーション
    validateFields(): boolean {
      let isValid = true;

      // タイトルのバリデーション
      if (!this.game.title || this.game.title.trim().length === 0) {
        this.titleError = 'タイトルを入力してください。';
        isValid = false;
      } else {
        this.titleError = null;
      }

      // メーカーのバリデーション
      if (!this.game.maker_Id || this.game.maker_Id === 0) {
        this.makerError = 'メーカーを選択してください。';
        isValid = false;
      } else {
        this.makerError = null;
      }

      // ジャンルのバリデーション
      if (!this.game.genre_Id || this.game.genre_Id === 0) {
        this.genreError = 'ジャンルを選択してください。';
        isValid = false;
      } else {
        this.genreError = null;
      }

      // 売上本数のバリデーション
      if (!this.game.sales_Count || this.game.sales_Count <= 0) {
        this.salesCountError = '売上件数を入力してください。';
        isValid = false;
      } else {
        this.salesCountError = null;
      }

      return isValid;
    }

    deleteGame(): void {
      const dialogRef = this.dialog.open(DeleteConfirmDialog, {
        width: '400px',
        data: {
          title: '削除確認',
          message: '以下のゲーム一覧を削除してもよろしいですか？',
          game: {
            title: this.game.title,
            maker: this.makers.find((m) => m.maker_Id === this.game.maker_Id)?.maker_Name ?? '不明',
            genre: this.genres.find((g) => g.genre_Id === this.game.genre_Id)?.genre_Name ?? '不明',
            salesCount: this.game.sales_Count,
            memo: this.game.memo ?? 'なし',
          },
        },
      });

      dialogRef.afterClosed().subscribe((result) => {
        if (result) {
          this.gamesService.deleteGame(this.game.game_Id!).subscribe({
            next: () => this.router.navigate(['/games'], { queryParams: this.queryParams }),
            error: () => {
              this.errorMessage = 'ゲームの削除に失敗しました。';
            },
          });
        }
      });
    }
  }

  // 削除確認モーダル
  @Component({
    selector: 'delete-confirm-dialog',
    template: `
      <h2 mat-dialog-title class="text-danger">{{ data.title }}</h2>
      <mat-dialog-content>
        <p>{{ data.message }}</p>
        <div class="game-details">
          <p><strong>タイトル:</strong> {{ data.game.title }}</p>
          <p><strong>メーカー:</strong> {{ data.game.maker }}</p>
          <p><strong>ジャンル:</strong> {{ data.game.genre }}</p>
          <p><strong>売上件数:</strong> {{ data.game.salesCount }} 件</p>
          <p><strong>備考欄:</strong> {{ data.game.memo }}</p>
        </div>
      </mat-dialog-content>
      <mat-dialog-actions align="end">
        <button mat-button mat-dialog-close>キャンセル</button>
        <button mat-raised-button color="warn" [mat-dialog-close]="true">削除</button>
      </mat-dialog-actions>
    `,
  })
  export class DeleteConfirmDialog {
    constructor(@Inject(MAT_DIALOG_DATA) public data: any) { }
  }
