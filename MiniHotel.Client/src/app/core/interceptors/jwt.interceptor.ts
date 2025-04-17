import { HttpInterceptorFn } from '@angular/common/http';

// Adds JWT token from localStorage to the Authorization header for every request
export const jwtInterceptor: HttpInterceptorFn = (req, next) => {
  const token = localStorage.getItem('jwtToken');
  if (token) {
    const clonedRequest = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`,
      },
    });
    return next(clonedRequest);
  }
  return next(req);
};
