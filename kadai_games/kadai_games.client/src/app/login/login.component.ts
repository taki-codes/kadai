import { Component } from '@angular/core';
import { LoginService } from '../services/login.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  email = '';
  password = '';
  errorMessage = '';

  constructor(private authService: LoginService, private router: Router) { }

  login(): void {
    this.authService.login(this.email, this.password).subscribe(
      (response) => {
        this.authService.setAdminStatus(response.isAdmin); // 管理者フラグを設定
        this.router.navigate(['/top']);
     },
      (error) => {
        this.errorMessage = 'Invalid login attempt';
      }
    );
  }
}
