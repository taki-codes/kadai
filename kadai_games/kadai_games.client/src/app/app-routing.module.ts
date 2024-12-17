import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './login/login.component';
import { TopComponent } from './top/top.component';
import { AuthGuard } from './guards/auth.guard';
import { GamesComponent } from './games/games.component';
import { GameComponent } from './game/game.component';
import { MastersComponent } from './masters/masters.component';
import { UsersComponent } from './users/users.component';
import { UserComponent } from './user/user.component';
import { GenreComponent } from './genre/genre.component';
import { GenresComponent } from './genres/genres.component';

const routes: Routes = [
  { path: '', redirectTo: '/login', pathMatch: 'full' }, // デフォルトルート
  { path: 'login', component: LoginComponent }, // ログインページ
  { path: 'top', component: TopComponent, canActivate: [AuthGuard] }, // トップページ
  { path: 'games', component: GamesComponent, canActivate: [AuthGuard] },//ゲーム一覧ページ
  { path: 'game/new', component: GameComponent, canActivate: [AuthGuard] },//ゲーム一覧：新規作成ページ
  { path: 'game/:id', component: GameComponent, canActivate: [AuthGuard] },//ゲーム一覧：詳細ページ
  { path: 'masters', component: MastersComponent, canActivate: [AuthGuard] },//各マスター管理遷移ページ
  { path: 'Users', component: UsersComponent, canActivate: [AuthGuard] },//ユーザーマスタ管理一覧ページ
  { path: 'user/new', component: UserComponent, canActivate: [AuthGuard] },//ユーザーマスタ管理一覧：新規登録ページ
  { path: 'user/:id', component: UserComponent, canActivate: [AuthGuard] },//ユーザーマスタ管理一覧：詳細ページ
  { path: 'Genres', component: GenresComponent, canActivate: [AuthGuard] },//ジャンルマスタ管理一覧ページ
  { path: 'genre/new', component: GenreComponent, canActivate: [AuthGuard] },//ジャンルマスタ管理一覧：新規登録ページ
  { path: 'genre/:id', component: GenreComponent, canActivate: [AuthGuard] },//ジャンルマスタ管理一覧：詳細ページ
 
];

@NgModule({
  imports: [RouterModule.forRoot(routes)], // ルーティング設定を適用
  exports: [RouterModule]
})
export class AppRoutingModule { }
