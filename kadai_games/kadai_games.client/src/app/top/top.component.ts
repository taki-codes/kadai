import { Component } from '@angular/core';
import { LoginService } from '../services/login.service';
import { ActivatedRoute, Router } from '@angular/router';


@Component({
  selector: 'app-top',
  templateUrl: './top.component.html',
  styleUrls: ['./top.component.css']
})
export class TopComponent {
  isAdmin: boolean = false;

  constructor(private authService: LoginService, private route: ActivatedRoute,
    private router: Router) { }

  ngOnInit(): void {

    this.authService.getUserInfo().subscribe(
      (response) => {
        this.isAdmin = response.isAdmin; // サーバーからフラグを再取得
      },
      (error) => {
        console.error('Failed to fetch user info:', error);
        this.router.navigate(['/login']);

        // エラー発生時にログイン画面へリダイレクト
      }
    );
  }
  // ゲーム一覧画面への遷移
  navigateToGames(): void {
    this.router.navigate(['/games'])
   
  }

  // マスター管理画面への遷移
  navigateToMaster(): void {
    this.router.navigate(['/master']);
  }

  //ログアウト処理
  logout(): void {
    this.authService.logout().subscribe(
      () => {
        this.router.navigate(['/login']); // Angular Router を利用
      },
      (error) => {
        alert('Logout failed');
      }
    );
  }
}
