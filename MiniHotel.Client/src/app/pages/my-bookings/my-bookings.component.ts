import { Component, OnInit } from '@angular/core';
import { BookingStatus, InvoiceDto, UserBookingsDto } from '../../api/models';
import { BookingsService, InvoicesService } from '../../api/services';
import { ToastrService } from 'ngx-toastr';
import { InvoiceSummaryComponent } from './invoice-summary/invoice-summary.component';
import { MatDialog } from '@angular/material/dialog';

@Component({
  selector: 'app-my-bookings',
  standalone: false,
  templateUrl: './my-bookings.component.html',
  styleUrl: './my-bookings.component.css'
})
export class MyBookingsComponent implements OnInit {
  bookings: UserBookingsDto[] = [];
  bookingStatusEnum = BookingStatus;

  constructor(
    private bookingsService: BookingsService,
    private invoiceService: InvoicesService,
    private toastr: ToastrService,
    private dialog: MatDialog
  ) { }

  ngOnInit(): void {
    this.loadBookings();
  }

  loadBookings(): void {
    this.bookingsService.getUserBookings().subscribe({
      next: (data) => {
        this.bookings = data.sort((a, b) =>
          new Date(b.startDate!).getTime() - new Date(a.startDate!).getTime()
        );
      },
      error: () => {
        this.toastr.error('Не вдалося завантажити ваші бронювання.');
      },
    });
  }

  cancelBooking(id: number): void {
    if (!confirm('Ви впевнені що хочете скасувати бронювання?')) {
      return;
    }

    this.bookingsService.cancelBooking({ id }).subscribe({
      next: () => {
        this.toastr.success('Бронювання скасовано.');
        this.loadBookings();
      },
      error: () => {
        this.toastr.error('Не вдалося скасувати бронювання.');
      }
    })
  }

  viewInvoice(bookingId: number): void {
    this.invoiceService.getInvoiceByBookingId({ bookingId })
      .subscribe((invoice: InvoiceDto) => {
        this.dialog.open(InvoiceSummaryComponent, {
          data: { invoice },
          width: '800px',
          maxWidth: 'none'
        });
      });
  }

  canCancel(status?: BookingStatus | string): boolean {
    return status === this.bookingStatusEnum.Pending || status === this.bookingStatusEnum.Confirmed;
  }

  getStatusClass(status: BookingStatus): string {
    switch (status) {
      case this.bookingStatusEnum.Pending:
        return 'text-warning';
      case this.bookingStatusEnum.Confirmed:
        return 'text-success';
      case this.bookingStatusEnum.Cancelled:
        return 'text-danger';
      default:
        return 'badge bg-light text-muted';
    }
  }
}
