import { Pipe, PipeTransform } from '@angular/core';
import { InvoiceStatus } from '../../../../api/models';

@Pipe({
  name: 'invoiceStatus',
  standalone: false
})
export class InvoiceStatusPipe implements PipeTransform {

  transform(value: InvoiceStatus | string): string {
    switch (value) {
      case InvoiceStatus.Paid:
        return 'Оплачено';
      case InvoiceStatus.Pending:
        return 'Очікує оплати';
      case InvoiceStatus.PartiallyPaid:
        return 'Частково оплачено';
      case InvoiceStatus.Cancelled:
        return 'Скасовано';
      case InvoiceStatus.Refunded:
        return 'Повернення коштів';
      case InvoiceStatus.Undefiend:
        return 'Невизначено';
      default:
        return value;
    }
  }
}
