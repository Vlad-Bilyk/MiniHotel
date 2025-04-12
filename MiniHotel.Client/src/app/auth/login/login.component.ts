import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthServiceWrapper } from '../services/auth.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-login',
  standalone: false,
  templateUrl: './login.component.html',
  styleUrl: './login.component.css',
})
export class LoginComponent implements OnInit {
  loginForm!: FormGroup;
  returnUrl: string = '/';
  showPassword = false;

  constructor(
    private fb: FormBuilder,
    private authService: AuthServiceWrapper,
    private route: ActivatedRoute,
    private router: Router,
    private toastr: ToastrService
  ) { }

  ngOnInit(): void {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', Validators.required],
    });

    this.returnUrl = this.route.snapshot.queryParamMap.get('returnUrl') ?? '/';
  }

  onSubmit(): void {
    if (this.loginForm.invalid) {
      return;
    }

    const loginData = this.loginForm.value;

    this.authService.login(loginData).subscribe({
      next: (result) => {
        if (result.success) {
          this.router.navigateByUrl(this.returnUrl);
        } else {
          this.toastr.error(result.errors?.[0] ?? 'Помилка входу');
        }
      },
      error: (err) => {
        this.toastr.error('Сталася помилка при вході');
        console.error(err);
      },
    });
  }
}
