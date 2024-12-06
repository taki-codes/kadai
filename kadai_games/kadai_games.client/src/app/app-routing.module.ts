import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './login/login.component';
import { TopComponent } from './top/top.component';
import { AuthGuard } from './guards/auth.guard';
import { GamesComponent } from './games/games.component';
import { GameComponent } from './game/game.component';

const routes: Routes = [
  { path: '', redirectTo: '/login', pathMatch: 'full' }, // デフォルトルート
  { path: 'login', component: LoginComponent }, // ログインページ
  { path: 'top', component: TopComponent, canActivate: [AuthGuard] }, // トップページ
  { path: 'games', component: GamesComponent, canActivate: [AuthGuard] },//ゲーム一覧ページ
  { path: 'game/new', component: GameComponent, canActivate: [AuthGuard] },//ゲーム一覧：新規作成ページ
  { path: 'game/:id', component: GameComponent, canActivate: [AuthGuard] },//ゲーム一覧：詳細ページ
  
];

@NgModule({
  imports: [RouterModule.forRoot(routes)], // ルーティング設定を適用
  exports: [RouterModule]
})
export class AppRoutingModule { }
