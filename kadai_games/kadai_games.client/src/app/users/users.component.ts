import { Component, OnInit } from '@angular/core';
import { MasterService } from '../services/master.service';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute, Router } from '@angular/router';
import { LoginService } from '../services/login.service';


@Component({
  selector: 'app-users',
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.css'],
})
export class UsersComponent implements OnInit {
  userId!: string;
  users: any[] = [];
  searchText: string = ''; // 検索文字列
  isAdmin: boolean | undefined; // 管理者フラグ
  pendingIsAdmin: boolean | undefined; // 一時的な管理者フラグ

  constructor(
    private masterService: MasterService,
    private loginService: LoginService,
    private http: HttpClient,
    private router: Router,
    private route: ActivatedRoute
  ) { }

  ngOnInit(): void {
    // クエリパラメータを取得して設定
    this.route.queryParams.subscribe((params) => {
      this.searchText = params['search'] || ''; // クエリから検索文字列を取得
      const isAdminParam = params['isAdmin'];

      // isAdmin の処理
      if (isAdminParam === undefined || isAdminParam === null) {
        this.isAdmin = undefined; // 全データ取得の条件
      } else {
        this.isAdmin = isAdminParam === 'true'; // クエリパラメータから変換
      }

      this.loadUsers(); // データをロード
    });
  }

  loadUsers(): void {
    // `isAdmin` が undefined または null の場合、全データを取得
    const adminFlag = this.isAdmin ?? undefined;

    this.masterService.getUsers(this.searchText.trim() || undefined, adminFlag).subscribe({
      next: (data) => {
        this.users = data;
        console.log('Users loaded:', this.users);
      },
      error: (error) => {
        console.error('Error fetching users:', error);
      },
    });
  }

  // 管理者フラグの切り替え
  toggleAdmin(event: Event): void {
    const inputElement = event.target as HTMLInputElement;
    this.pendingIsAdmin = inputElement.checked ? true : undefined; // 一時的に保存
  }

  // 検索メソッド
  search(): void {
    this.isAdmin = this.pendingIsAdmin; // 一時的な状態を反映
    this.updateQueryParams(); // クエリパラメータを更新
  }

  // クエリパラメータを更新
  updateQueryParams(): void {
    this.router.navigate([], {
      relativeTo: this.route, // 現在のルートを基準
      queryParams: {
        search: this.searchText.trim() || null, // 検索文字列
        isAdmin: this.isAdmin ?? null, // 管理者フラグ
      },
      queryParamsHandling: 'merge', // 既存のクエリパラメータをマージ
    });
  }

  // 詳細画面に遷移
  navigateToDetail(userId: string): void {
    this.router.navigate(['/user', userId], {
      queryParams: {
        search: this.searchText.trim() || null, // 検索文字列を引き継ぐ
        isAdmin: this.isAdmin ?? null, // 管理者フラグを引き継ぐ
      },
    });
  }

  // 登録画面に遷移
  navigateToRegister(): void {
    this.router.navigate(['user/new']);
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
