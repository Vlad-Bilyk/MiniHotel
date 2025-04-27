import { Pipe, PipeTransform } from '@angular/core';
import { PaymentMethod } from '../../../../api/models';

@Pipe({
  name: 'paymentMethod',
  standalone: false
})
export class PaymentMethodPipe implements PipeTransform {

  transform(value: PaymentMethod | string): string {
    switch (value) {
      case PaymentMethod.Online:
        return 'Онлайн';
      case PaymentMethod.OnSite:
        return 'На місці';
      default:
        return value;
    }
  }
}
