import { CanActivateFn, Router } from '@angular/router';
import { AuthServiceWrapper } from '../../auth/services/auth.service';
import { inject } from '@angular/core';
import { ToastrService } from 'ngx-toastr';

export const roleGuard = (expectedRoles: string[]): CanActivateFn => {
  return () => {
    const auth = inject(AuthServiceWrapper);
    const toastr = inject(ToastrService);
    const router = inject(Router);

    const hasRole = auth.hasAnyRole(expectedRoles);

    if (!hasRole) {
      toastr.error('У вас немає доступу до цієї сторінки');
      router.navigate(['/']);
      return false;
    }

    return true;
  }
};
