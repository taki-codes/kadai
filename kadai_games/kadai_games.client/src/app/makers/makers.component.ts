import { Component, OnInit } from '@angular/core';
import { MasterService } from '../services/master.service';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute, Router } from '@angular/router';
import { LoginService } from '../services/login.service';

@Component({
  selector: 'app-makers',
  templateUrl: './makers.component.html',
  styleUrls: ['./makers.component.css'],
})
export class MakersComponent implements OnInit {
  makers: any[] = [];
  searchText: string = '';

  constructor(
    private masterService: MasterService,
    private loginService: LoginService,
    private router: Router,
    private route: ActivatedRoute
  ) { }

  ngOnInit(): void {
    this.route.queryParams.subscribe((params) => {
      this.searchText = params['search'] || '';
      this.loadMakers();
    });
  }

  // メーカーデータを取得
  loadMakers(): void {
    const searchText = this.searchText?.trim() || undefined;

    this.masterService.getMakers(searchText).subscribe({
      next: (data) => (this.makers = data),
      error: (err) => console.error('エラーが発生しました:', err),
    });
  }

  // 検索処理を実行し、クエリパラメータを更新
  search(): void {
    this.updateQueryParams();
  }

  // クエリパラメータを更新
  updateQueryParams(): void {
    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: { search: this.searchText.trim() || '' },
      queryParamsHandling: 'merge',
    });
  }

  // 新規作成画面に遷移
  navigateToNewMaker(): void {
    this.router.navigate(['/maker/new']);
  }

  // 詳細画面に遷移
  navigateToDetail(makerId: number): void {
    this.router.navigate(['/maker', makerId], {
      queryParams: { search: this.searchText.trim() || '' },
      queryParamsHandling: 'merge',
    });
  }

  // トップ画面に遷移
  navigateToTop(): void {
    this.router.navigate(['/top']);
  }

  // ログアウト処理
  logout(): void {
    this.loginService.logout().subscribe({
      next: () => {
        this.loginService.setAdminStatus(false);
        this.router.navigate(['/login']);
      },
      error: () => alert('ログアウトに失敗しました'),
    });
  }
}
