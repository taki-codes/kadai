import { Component } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { LoginService } from '../services/login.service';

@Component({
  selector: 'app-masters',
  templateUrl: './masters.component.html',
  styleUrl: './masters.component.css'
})
export class MastersComponent {
  constructor(private authService: LoginService, private route: ActivatedRoute,
    private router: Router) { }
 

  // ユーザーマスター管理画面への遷移
  navigateToUsers(): void {
    this.router.navigate(['/Users'])

  }

  // メーカーマスター管理画面への遷移
  navigateToMakers(): void {
    this.router.navigate(['/Makers']);
  }

  // ジャンルマスター画面画面への遷移
  navigateToGenres(): void {
    this.router.navigate(['/Genres'])

  }

  // Top画面に遷移
  navigateToTop(): void {
    this.router.navigate(['/top']);
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
