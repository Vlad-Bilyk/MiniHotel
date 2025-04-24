import { HttpInterceptorFn } from '@angular/common/http';
import { AuthServiceWrapper } from '../../auth/services/auth.service';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';

export const authExpirationInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthServiceWrapper);
  return next(req).pipe(
    catchError((err) => {
      if (err.status === 401) {
        authService.logout();
      }
      return throwError(() => err);
    })
  );
};
