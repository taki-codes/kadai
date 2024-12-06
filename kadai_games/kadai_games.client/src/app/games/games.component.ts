import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { GamesService } from '../services/games.service';
import { LoginService } from '../services/login.service';

interface Game {
  game_Id: number;
  title: string;
  maker_Name: string;
  genre_Name: string;
  sales_Count: number;
}

@Component({
  selector: 'app-games',
  templateUrl: './games.component.html',
  styleUrls: ['./games.component.css']
})
export class GamesComponent implements OnInit {
  games: Game[] = [];
  filteredGames: Game[] = [];
  searchQuery: string = '';

  sortOrder: 'asc' | 'desc' = 'asc'; // ソート順
  errorMessage: string | null = null;
  isLoading: boolean = false;

  constructor(
    private gamesService: GamesService,
    private loginService: LoginService,
    private router: Router,
    private route: ActivatedRoute
  ) { }

  ngOnInit(): void {
    // クエリパラメータを取得して状態を復元
    this.route.queryParams.subscribe(params => {
      this.searchQuery = params['word'] || '';

      this.sortOrder = params['order'] || 'asc';
      this.loadGames();
    });
  }

  // ゲーム一覧をロード
  loadGames(): void {
    this.isLoading = true;
    this.errorMessage = null;

    // サーバーからデータ取得
    this.gamesService.getGames().subscribe({
      next: (data: Game[]) => {
        this.games = data;
        this.applyFilters(); // フィルタとソートを適用
        this.isLoading = false;
      },
      error: (error) => {
        console.error('ゲーム一覧の取得に失敗しました:', error);
        this.isLoading = false;
      }
    });
  }


  applyFilters(): void {
    // フィルタリング処理
    this.filteredGames = this.games.filter(game =>
      (game.title?.toLowerCase() ?? '').includes(this.searchQuery.toLowerCase()) ||
      (game.maker_Name?.toLowerCase() ?? '').includes(this.searchQuery.toLowerCase()) ||
      (game.genre_Name?.toLowerCase() ?? '').includes(this.searchQuery.toLowerCase())
    );

    // フィルタ後にソートを適用
    this.sortData();
  }


  // データをソート
  sortData(): void {
    this.filteredGames.sort((a, b) => {
      return this.sortOrder === 'asc'
        ? a.sales_Count - b.sales_Count
        : b.sales_Count - a.sales_Count;
    });

    // URL を更新してソート情報を反映
    this.updateQueryParams();
  }

  updateQueryParams(): void {
    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: {
        word: this.searchQuery || '',

        order: this.sortOrder || 'asc',
      },
      queryParamsHandling: 'merge',
    });
  }
  // 検索クエリの変更
  onSearchChange(): void {
    this.applyFilters();
  }


  // ソート基準の変更
  onSortChange(field: string, order: 'asc' | 'desc'): void {
    this.sortOrder = order;

    // ソートを適用してURLを更新
    this.sortData();
  }

  // 詳細画面に遷移
  navigateToDetail(gameId: number): void {
    // 詳細画面に遷移（クエリパラメータを引き継ぐ）
    this.router.navigate(['/game', gameId], {
      queryParams: {
        word: this.searchQuery || '',

        order: this.sortOrder || 'asc'
      }
    });
  }
  // 新規作成画面に遷移
  navigateToNewGame(): void {
    this.router.navigate(['/game/new'], {
      queryParams: {
        word: this.searchQuery || '',

        order: this.sortOrder || 'asc'
      }
    });
  }

  // Top画面に遷移
  navigateToTop(): void {
    this.router.navigate(['/top']);
  }


  // ログアウト処理
  logout(): void {
    this.loginService.logout().subscribe(
      () => {
        this.loginService.setAdminStatus(false);
        this.router.navigate(['/login']);
      },
      (error) => {
        alert('ログアウトに失敗しました');
      }
    );
  }
}
