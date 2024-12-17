import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class GamesService {
  // デフォルトのソート順
  sortOrder: 'asc' | 'desc' = 'asc';

  // API のベース URL
  private baseUrl = 'https://localhost:7094/api/Game';

  constructor(private http: HttpClient) { }

  /**
   * ゲーム一覧を取得
   * @param query クエリパラメータ（任意）
   * @returns Observable
   */
  getGames(query: any = {}): Observable<any> {
    return this.http.get(`${this.baseUrl}`, { params: query });
  }

  /**
   * 指定された ID のゲームを取得
   * @param id ゲームの ID
   * @returns Observable
   */
  getGameById(id: number): Observable<any> {
    return this.http.get(`${this.baseUrl}/${id}`);
  }

  /**
   * ゲーム一覧を新規作成
   * @param game ゲームデータ
   * @returns Observable
   */
  createGame(game: any): Observable<any> {
    return this.http.post(this.baseUrl, game);
  }

  /**
   * 一覧データを更新
   * @param id ゲームの ID
   * @param game 更新するゲームデータ
   * @returns Observable
   */
  updateGame(id: number, game: any): Observable<any> {
    console.log('Payload being sent:', game); // デバッグログで確認
    return this.http.put(`${this.baseUrl}/${id}`, game);
  }

  /**
   * 一覧データを削除
   * @param id ゲームの ID
   * @returns Observable
   */
  deleteGame(id: number): Observable<any> {
    return this.http.delete(`${this.baseUrl}/${id}`);
  }

  /**
   * メーカー一覧を取得
   * @returns Observable
   */
  getMakers(): Observable<any> {
    return this.http.get(`${this.baseUrl}/makers`);
  }

  /**
   * ジャンル一覧を取得
   * @returns Observable
   */
  getGenres(): Observable<any> {
    return this.http.get(`${this.baseUrl}/genres`);
  }
}
