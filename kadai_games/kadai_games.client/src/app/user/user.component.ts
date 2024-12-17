  import { Component, OnInit } from '@angular/core';
  import { ActivatedRoute, Router } from '@angular/router';
  import { MasterService } from '../services/master.service';
  import { FormBuilder, FormGroup, Validators } from '@angular/forms';

  @Component({
    selector: 'app-user',
    templateUrl: './user.component.html',
    styleUrls: ['./user.component.css'],
  })
  export class UserComponent implements OnInit {
    registerForm!: FormGroup;
    userId!: string;
    user: any;
    userForm!: FormGroup;
    queryParams: any = {}; 
    errorMessage: string | null = null;


    constructor(
      private route: ActivatedRoute,
      private router: Router,
      private masterService: MasterService,
      private fb: FormBuilder
    ) { }

    ngOnInit(): void {

      this.initializeRegisterForm();

      // クエリパラメータを取得して保存
      this.route.queryParams.subscribe((params) => {
        this.queryParams = params;
        console.log('クエリパラメータ:', this.queryParams);
      });

      // ルートパラメータからユーザーIDを取得
      this.userId = this.route.snapshot.paramMap.get('id') || '';
      console.log('User ID:', this.userId);

      if (!this.userId) {
        console.error('Error: User ID is undefined or empty');
        return;
      }

      // ユーザー詳細を取得
      this.masterService.getUserById(this.userId).subscribe(
        (data) => {
          this.user = data;
          this.initializeForm();
        },
        (error) => console.error('Error fetching user details:', error)
      );
    }

    // 登録フォームの初期化
    initializeRegisterForm(): void {
      this.registerForm = this.fb.group({
        userName: ['', [Validators.required, Validators.minLength(3)]], // ユーザー名必須かつ3文字以上
        email: ['', [Validators.required, Validators.email]], // メールアドレス必須かつ形式チェック
        password: ['', [Validators.required, Validators.minLength(6)]], // パスワード必須かつ6文字以上
        isAdmin: [false],
      });
    }

    registerUser(): void {
      if (this.registerForm.valid) {
        this.masterService.registerUser(this.registerForm.value).subscribe({
          next: () => {
            this.router.navigate(['/Users']);
          },
          error: (err) => {
            this.errorMessage = err.errors || 'エラーが発生しました。詳細はありません。';
          },
        });
      } else {
        this.markFormGroupTouched(this.registerForm);
      }
    }

    // サーバーからのエラーをフォームコントロールに反映
    private displayErrors(errors: any): void {
      Object.keys(errors).forEach((field) => {
        const control = this.registerForm.get(field);
        if (control) {
          control.setErrors({ server: errors[field].join(' ') });
        }
      });
    }

    // フォーム内のすべてのフィールドを「触れた」状態にする
    private markFormGroupTouched(formGroup: FormGroup): void {
      Object.values(formGroup.controls).forEach((control) => {
        if (control instanceof FormGroup) {
          this.markFormGroupTouched(control);
        } else {
          control.markAsTouched();
        }
      });
    }
    
    // フォームを初期化
    initializeForm(): void {
      this.userForm = this.fb.group({
        userName: [
          this.user?.userName,
          [Validators.required] // 必須かつ3文字以上
        ],
        email: [
          this.user?.email,
          [Validators.required, Validators.email] // 必須かつ有効なメールアドレス
        ],
        isAdminFlg: [this.user?.isAdminFlg],
      });
    }

    // ユーザーを削除
    deleteUser(): void {
      if (confirm('このユーザーを削除してもよろしいですか？')) {
        this.masterService.deleteUser(this.userId).subscribe(
          (response) => {
            alert('ユーザーが削除されました');
            this.router.navigate(['/Users']); 
          },
          (error) => console.error(error)
        );
      }
    }
    // リスト画面に戻る
    goBack(): void {
      this.router.navigate(['/Users'], {
        queryParams: this.queryParams, // クエリパラメータを保持してリスト画面に戻る
      });
    }


    updateUser(): void {
      if (this.userForm.valid) {
        this.masterService.updateUser(this.userId, this.userForm.value).subscribe(
          (response) => {
            alert('ユーザーが更新されました');
            this.router.navigate(['/Users']);
          },
          (error) => console.error(error)
        );
      } else {
        this.markFormGroupTouched(this.userForm);
      }
    }

  }
  

  
