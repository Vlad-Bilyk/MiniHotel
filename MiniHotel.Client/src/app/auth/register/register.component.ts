import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AuthServiceWrapper } from '../services/auth.service';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-register',
  standalone: false,
  templateUrl: './register.component.html',
  styleUrl: './register.component.css',
})
export class RegisterComponent implements OnInit {
  registerForm!: FormGroup;
  isSubmitting = false;
  isManager = false;

  constructor(
    private fb: FormBuilder,
    private authService: AuthServiceWrapper,
    private router: Router,
    private toastr: ToastrService
  ) { }

  ngOnInit(): void {
    this.registerForm = this.fb.group(
      {
        firstName: ['', Validators.required],
        lastName: ['', Validators.required],
        email: [
          '',
          Validators.compose([Validators.required, Validators.email]),
        ],
        phoneNumber: [
          '',
          Validators.compose([
            Validators.required,
            Validators.pattern(/^\+?\d{7,15}$/),
          ]),
        ],
        role: [this.isManager ? 'Customer' : 'Customer'],
        password: [
          '',
          Validators.compose([
            Validators.required,
            Validators.minLength(6),
            Validators.pattern(/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).+$/),
          ]),
        ],
        confirmPassword: ['', Validators.required],
      },
      { validators: this.passwordsMatchValidator }
    );
  }

  passwordsMatchValidator(form: FormGroup) {
    return form.get('password')?.value === form.get('confirmPassword')?.value
      ? null
      : { mismatch: true };
  }

  onSubmit(): void {
    if (this.registerForm.invalid) return;

    this.isSubmitting = true;

    this.authService.register(this.registerForm.value).subscribe({
      next: (res) => {
        if (res.success && res.token) {
          this.toastr.success('Реєстрація успішна!');
          this.router.navigate(['/']);
        } else {
          this.toastr.error(res.errors?.[0] ?? 'Помилка при реєстрації');
        }
        this.isSubmitting = false;
      },
      error: (err) => {
        this.toastr.error('Сталася помилка при реєстрації');
        console.error(err);
        this.isSubmitting = false;
      },
    });
  }
}
