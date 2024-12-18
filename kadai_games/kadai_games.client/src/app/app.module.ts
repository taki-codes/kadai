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
import { ReactiveFormsModule } from '@angular/forms';
import { MastersComponent } from './masters/masters.component';
import { UsersComponent } from './users/users.component';
import { UserComponent } from './user/user.component'; 
import { MasterService } from './services/master.service';
import { CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { GenresComponent } from './genres/genres.component';
import { GenreComponent } from './genre/genre.component';
import { MakersComponent } from './makers/makers.component';
import { MakerComponent } from './maker/maker.component';

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    TopComponent,
    GamesComponent,
    DeleteConfirmDialog, 
    GameComponent,
    MastersComponent,
    UsersComponent,
    UserComponent,
    GenresComponent,
    GenreComponent,
    MakersComponent,
    MakerComponent, 
  ],

  imports: [
    BrowserModule,
    AppRoutingModule,
    FormsModule,
    HttpClientModule,
    CommonModule,
    RouterModule,
    BrowserAnimationsModule,
    MatDialogModule,
    MatButtonModule,
    ReactiveFormsModule, 
    FormsModule 
  ],
  providers: [
    LoginService,
    GamesService,
    MasterService,
  ],
  bootstrap: [AppComponent],
})

export class AppModule { }
