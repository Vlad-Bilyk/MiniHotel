import { Pipe, PipeTransform } from '@angular/core';
import { UserRole } from '../../../../api/models';

@Pipe({
  name: 'userRole',
  standalone: false
})
export class UserRolePipe implements PipeTransform {

  transform(value: UserRole | string): string {
    switch (value) {
      case UserRole.Client:
        return 'Клієнт';
      case UserRole.Receptionist:
        return 'Адміністратор рецепсії';
      case UserRole.Manager:
        return 'Менеджер';
      default:
        return value;
    }
  }

}
