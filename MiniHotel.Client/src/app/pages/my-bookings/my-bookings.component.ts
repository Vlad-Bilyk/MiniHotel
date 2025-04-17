import { Component, OnInit } from '@angular/core';
import { BookingStatus, UserBookingsDto } from '../../api/models';
import { BookingsService } from '../../api/services';
import { ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';

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
    private toastr: ToastrService,
    private router: Router
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

  // TODO: Inmplemet in future
  viewInvoice(invoiceId: number): void {
    this.toastr.info(`Функціонал перегляду рахунку ще не реалізовано (ID: ${invoiceId})`);
    console.log('Invoice ID:', invoiceId);
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
