import { Injectable } from '@angular/core';
import { AuthService } from '../../api/services';
import {
  AuthenticationResultDto,
  LoginRequestDto,
  RegisterRequestDto,
} from '../../api/models';
import { Observable, tap } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class AuthServiceWrapper {
  private readonly tokenKey = 'jwtToken';

  constructor(private apiAuthService: AuthService) {}

  login(loginData: LoginRequestDto): Observable<AuthenticationResultDto> {
    return this.apiAuthService.authLogin({ body: loginData }).pipe(
      tap((result) => {
        if (result.success && result.token) {
          localStorage.setItem(this.tokenKey, result.token);
        }
      })
    );
  }

  register(
    registerData: RegisterRequestDto
  ): Observable<AuthenticationResultDto> {
    return this.apiAuthService.authRegister({ body: registerData }).pipe(
      tap((result) => {
        if (result.success && result.token) {
          localStorage.setItem(this.tokenKey, result.token);
        }
      })
    );
  }

  isLoggedIn(): boolean {
    return !!localStorage.getItem(this.tokenKey);
  }

  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  logout(): void {
    localStorage.removeItem(this.tokenKey);
  }
}
