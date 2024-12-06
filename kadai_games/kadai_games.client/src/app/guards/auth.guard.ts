import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { LoginService } from '../services/login.service';
import { Observable, of } from 'rxjs';
import { catchError, map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  constructor(private authService: LoginService, private router: Router) { }

  canActivate(): Observable<boolean> {
    return this.authService.getUserInfo().pipe(
      map((response) => {
        // ユーザー情報が取得できた場合
        return true; // アクセス許可
      }),
      catchError((error) => {
        this.router.navigate(['/login']); // ユーザー情報が取得できない場合ログイン画面にリダイレクト
        return of(false); // アクセス拒否
      })
    );
  }
}
