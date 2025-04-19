import { Injectable } from '@angular/core';
import { AuthService } from '../../api/services';
import {
  AuthenticationResultDto,
  LoginRequestDto,
  RegisterRequestDto,
} from '../../api/models';
import { BehaviorSubject, Observable, tap } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class AuthServiceWrapper {
  private readonly tokenKey = 'jwtToken';

  private userRoleSubject = new BehaviorSubject<string | null>(
    this.getUserRole()
  );
  public userRole$: Observable<string | null> =
    this.userRoleSubject.asObservable();

  constructor(private apiAuthService: AuthService) { }

  login(loginData: LoginRequestDto): Observable<AuthenticationResultDto> {
    return this.apiAuthService
      .login({ body: loginData })
      .pipe(tap((result) => this.handleAuthSuccess(result)));
  }

  register(
    registerData: RegisterRequestDto
  ): Observable<AuthenticationResultDto> {
    return this.apiAuthService
      .register({ body: registerData })
      .pipe(tap((result) => this.handleAuthSuccess(result)));
  }

  isLoggedIn(): boolean {
    return !!localStorage.getItem(this.tokenKey);
  }

  logout(): void {
    localStorage.removeItem(this.tokenKey);
    this.userRoleSubject.next(null);
  }

  hasRole(role: string): boolean {
    return this.getUserRole() === role;
  }

  hasAnyRole(roles: string[]): boolean {
    const userRole = this.getUserRole();
    return !!userRole && roles.includes(userRole);
  }

  private getUserRole(): string | null {
    const token = this.getToken();
    if (!token) return null;

    const payload = this.parseJwt(token);
    return (
      payload?.[
      'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'
      ] ?? null
    );
  }

  private getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  private handleAuthSuccess(result: AuthenticationResultDto): void {
    if (result.success && result.token) {
      localStorage.setItem(this.tokenKey, result.token);
      this.userRoleSubject.next(this.getUserRole());
    }
  }

  private parseJwt(token: string): any {
    try {
      return JSON.parse(atob(token.split('.')[1]));
    } catch {
      return null;
    }
  }
}
