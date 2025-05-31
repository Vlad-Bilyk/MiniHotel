import { Injectable } from '@angular/core';
import { BookingStatus, InvoiceStatus } from '../../api/models';

@Injectable({
  providedIn: 'root'
})
export class StatusStyleService {

  getBookingStatusClass(status: BookingStatus | string): string {
    switch (status) {
      case BookingStatus.Pending:
        return 'chip-pending';
      case BookingStatus.Confirmed:
        return 'chip-confirmed';
      case BookingStatus.Cancelled:
        return 'chip-cancelled';
      case BookingStatus.CheckedIn:
        return 'chip-checked-in';
      case BookingStatus.CheckedOut:
        return 'chip-checked-out';
      default:
        return 'chip-default';
    }
  }

  getInvoiceStatusClass(status: InvoiceStatus | string): string {
    switch (status) {
      case InvoiceStatus.Paid:
        return 'chip-confirmed';
      case InvoiceStatus.Pending:
        return 'chip-pending';
      case InvoiceStatus.PartiallyPaid:
        return 'chip-pending';
      case InvoiceStatus.Cancelled:
        return 'chip-cancelled';
      case InvoiceStatus.Refunded:
        return 'chip-cancelled';
      case InvoiceStatus.Undefined:
        return 'chip-default';
      default:
        return 'chip-default';
    }
  }
}
