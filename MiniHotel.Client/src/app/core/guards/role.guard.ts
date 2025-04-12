import { CanActivateFn } from '@angular/router';
import { AuthServiceWrapper } from '../../auth/services/auth.service';
import { inject } from '@angular/core';

export const roleGuard = (expectedRoles: string[]): CanActivateFn => {
  return () => {
    const auth = inject(AuthServiceWrapper);
    return auth.hasAnyRole(expectedRoles);
  }
};
