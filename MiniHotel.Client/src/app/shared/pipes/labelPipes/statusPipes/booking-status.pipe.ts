import { Pipe, PipeTransform } from '@angular/core';
import { BookingStatus } from '../../../../api/models';

@Pipe({
  name: 'bookingStatus',
  standalone: false
})
export class BookingStatusPipe implements PipeTransform {

  transform(value: BookingStatus | string): string {
    switch (value) {
      case BookingStatus.Pending:
        return 'Очікує підтвердження';
      case BookingStatus.Confirmed:
        return 'Підтверджено';
      case BookingStatus.CheckedIn:
        return 'Заселений';
      case BookingStatus.CheckedOut:
        return 'Завершено';
      case BookingStatus.Cancelled:
        return 'Скасовано';
      default:
        return value;
    }
  }

}
