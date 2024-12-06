import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class LoginService {
  // API のベース URL
  private apiUrl = 'https://localhost:7094/api/account';

  // 管理者フラグ
  isAdmin: boolean = false;

  constructor(private http: HttpClient) { }

  /**
   * ログイン処理
   * @param email ユーザーのメールアドレス
   * @param password ユーザーのパスワード
   * @returns Observable
   */
  login(email: string, password: string): Observable<any> {
    const body = { email, password };
    return this.http.post(`${this.apiUrl}/login`, body, { withCredentials: true });
  }

  /**
   * ログアウト処理
   * @returns Observable
   */
  logout(): Observable<any> {
    return this.http.post(`${this.apiUrl}/logout`, {}, { withCredentials: true });
  }

  /**
   * 管理者フラグを設定
   * @param isAdmin 管理者フラグ（true: 管理者, false: 一般ユーザー）
   */
  setAdminStatus(isAdmin: boolean): void {
    this.isAdmin = isAdmin;
  }

  /**
   * 管理者フラグを取得
   * @returns boolean 管理者フラグの状態
   */
  getAdminStatus(): boolean {
    return this.isAdmin;
  }

  /**
   * ログインユーザー情報を取得
   * @returns Observable
   */
  getUserInfo(): Observable<any> {
    return this.http.get(`${this.apiUrl}/userinfo`, { withCredentials: true });
  }
}
