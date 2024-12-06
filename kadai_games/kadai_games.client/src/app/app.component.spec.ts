import { Component } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'My Angular Application';

  constructor() {
    // アプリ起動時に実行する初期処理
    console.log('Application started');
  }
}
