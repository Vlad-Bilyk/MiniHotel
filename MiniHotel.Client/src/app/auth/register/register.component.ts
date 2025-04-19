import { Component, OnInit } from '@angular/core';
import {
  AbstractControl,
  FormBuilder,
  FormGroup,
  ValidationErrors,
  Validators,
} from '@angular/forms';
import { AuthServiceWrapper } from '../services/auth.service';
import { ActivatedRoute } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Location } from '@angular/common';
import { finalize, take } from 'rxjs';
import { UserRole } from '../../api/models';

@Component({
  selector: 'app-register',
  standalone: false,
  templateUrl: './register.component.html',
  styleUrl: './register.component.css',
})
export class RegisterComponent implements OnInit {
  registerForm!: FormGroup;
  isSubmitting = false;
  showRoleSelected = false;
  roleOptions = Object.values(UserRole);

  get formControls() {
    return this.registerForm.controls;
  }

  constructor(
    private fb: FormBuilder,
    private authService: AuthServiceWrapper,
    private route: ActivatedRoute,
    private toastr: ToastrService,
    private location: Location
  ) { }

  ngOnInit(): void {
    this.buildForm();
    this.checkQueryRole();
  }

  onSubmit(): void {
    if (this.registerForm.invalid) {
      this.registerForm.markAllAsTouched();
      return;
    }

    this.isSubmitting = true;
    this.authService
      .register(this.registerForm.value)
      .pipe(finalize(() => (this.isSubmitting = false)))
      .subscribe({
        next: (res) => {
          if (res.success && res.token) {
            this.toastr.success('Реєстрація успішна!');
            this.location.back();
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

  private passwordsMatchValidator(
    group: AbstractControl
  ): ValidationErrors | null {
    const pass = group.get('password')?.value;
    const confirm = group.get('confirmPassword')?.value;
    return pass === confirm ? null : { mismatch: true };
  }

  private buildForm() {
    this.registerForm = this.fb.group(
      {
        firstName: ['', [Validators.required, Validators.maxLength(50)]],
        lastName: ['', [Validators.required, Validators.maxLength(50)]],
        email: ['', [Validators.required, Validators.email]],
        phoneNumber: [
          '',
          [Validators.required, Validators.pattern(/^\+?\d{7,15}$/)],
        ],
        role: ['Client'],
        password: [
          '',
          [
            Validators.required,
            Validators.minLength(6),
            Validators.pattern(/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).+$/),
          ],
        ],
        confirmPassword: ['', Validators.required],
      },
      { validators: this.passwordsMatchValidator }
    );
  }

  private checkQueryRole() {
    this.route.queryParamMap.pipe(take(1)).subscribe(params => {
      const isManagerParam = params.get('isManager') === 'true';
      this.showRoleSelected = isManagerParam;
    })
  }
}
