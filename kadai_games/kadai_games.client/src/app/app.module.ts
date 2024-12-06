import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { HttpClientModule} from '@angular/common/http';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { LoginComponent } from './login/login.component';
import { LoginService } from './services/login.service';
import { CommonModule } from '@angular/common';
import { TopComponent } from './top/top.component';
import { RouterModule } from '@angular/router';
import { GamesComponent } from './games/games.component';
import { GameComponent, } from './game/game.component';
import { MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { DeleteConfirmDialog } from './game/game.component';
import { GamesService } from './services/games.service';
import { ReactiveFormsModule } from '@angular/forms'; // ReactiveFormsModuleを追加

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    TopComponent,
    GamesComponent,
    DeleteConfirmDialog, 
    GameComponent, 
   
  ],

  imports: [
    BrowserModule,
    AppRoutingModule,
    FormsModule,
    HttpClientModule,
    CommonModule,
    RouterModule,
    RouterModule.forRoot([]), 
    BrowserAnimationsModule,
    MatDialogModule,
    MatButtonModule,
    ReactiveFormsModule, // ReactiveFormsModuleをインポート
  ],
  providers: [
    LoginService,
    GamesService,
  ],

  bootstrap: [AppComponent], 
})
export class AppModule { }
