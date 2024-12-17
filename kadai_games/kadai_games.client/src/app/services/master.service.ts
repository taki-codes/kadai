import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { catchError, Observable, tap, throwError } from 'rxjs';



@Injectable({
  providedIn: 'root'
})
export class MasterService {
  private baseUrl_users = 'https://localhost:7094/api/User';
  private baseUrl_genres = 'https://localhost:7094/api/Genre'; 

  constructor(private http: HttpClient) { }

  //一覧画面取得＆検索
  getUsers(searchText?: string, isAdmin?: boolean): Observable<any[]> {
    let params: any = {};
    if (searchText) { params.searchText = searchText; }
    if (isAdmin !== undefined) { params.isAdmin = isAdmin; } return this.http.get<any[]>(`${this.baseUrl_users}/users`, { params });
  }
  // ユーザー詳細取得
  getUserById(id: string): Observable<any> {
    return this.http.get(`${this.baseUrl_users}/users/${id}`);
  }

  // ユーザー削除
  deleteUser(id: string): Observable<any> {
    return this.http.delete(`${this.baseUrl_users}/users/${id}`);
  }

  // ユーザー更新
  updateUser(id: string, data: any): Observable<any> {
    return this.http.put(`${this.baseUrl_users}/users/${id}`, data);
  }

  // 新規登録処理
  registerUser(user: any): Observable<any> {
    return this.http.post(`${this.baseUrl_users}/register`, user).pipe(
      catchError((error) => {
        return throwError(error.error); // サーバーからのエラーメッセージをスロー
      })
    );
  }
  //一覧画面取得＆検索(ジャンルマスタ)
  getGenres(searchText?: string): Observable<any> {
    let params: any = {};
    if (searchText) { params.searchText = searchText; }
    return this.http.get<any[]>(`${this.baseUrl_genres}/genres`, { params });
  }
  // 新規登録処理(ジャンルマスタ)
  createGenre(genre: any): Observable<any> {
    return this.http.post(this.baseUrl_genres, genre);
  }
  // ユーザー詳細取得(ジャンルマスタ)
  getGenreById(id: number): Observable<any> {
    return this.http.get<any>(`${this.baseUrl_genres}/genres/${id}`);
  }

  // ユーザー削除(ジャンルマスタ)
  deleteGenre(id: number): Observable<any> {
    return this.http.delete(`${this.baseUrl_genres}/genres/${id}`);
  }

  // ユーザー更新(ジャンルマスタ)
  updateGenre(id: number, updatedGenre: any): Observable<any> {
    return this.http.put(`${this.baseUrl_genres}/genres/${id}`, updatedGenre);
  }
  
}
