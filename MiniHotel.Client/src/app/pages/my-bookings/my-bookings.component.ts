import { Component, OnInit } from '@angular/core';
import { BookingStatus, InvoiceDto, UserBookingsDto } from '../../api/models';
import { BookingsService, InvoicesService } from '../../api/services';
import { ToastrService } from 'ngx-toastr';
import { InvoiceSummaryComponent } from './invoice-summary/invoice-summary.component';
import { MatDialog } from '@angular/material/dialog';
import { StatusStyleService } from '../../shared/services/status-style.service';
import { PageEvent } from '@angular/material/paginator';

@Component({
  selector: 'app-my-bookings',
  standalone: false,
  templateUrl: './my-bookings.component.html',
  styleUrl: './my-bookings.component.css'
})
export class MyBookingsComponent implements OnInit {
  displayedColumns: string[] = ['roomCategory', 'startDate', 'endDate', 'amount', 'status', 'actions'];

  bookings: UserBookingsDto[] = [];
  bookingStatusEnum = BookingStatus;

  totalCount = 0;
  pageSize = 10;
  pageIndex = 0;

  constructor(
    private statusStyleService: StatusStyleService,
    private bookingsService: BookingsService,
    private invoiceService: InvoicesService,
    private toastr: ToastrService,
    private dialog: MatDialog,
  ) { }

  ngOnInit(): void {
    this.loadBookings();
  }

  loadBookings(): void {
    this.bookingsService.getUserBookings({
      pageNumber: this.pageIndex + 1,
      pageSize: this.pageSize
    }).subscribe({
      next: (result) => {
        this.bookings = result.items ?? [];
        this.totalCount = result.totalCount!;
      },
      error: (err) => {
        console.error(err);
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

  getBookingChipClass(status: BookingStatus | string): string {
    return this.statusStyleService.getBookingStatusClass(status);
  }

  pageChanged(e: PageEvent): void {
    this.pageIndex = e.pageIndex;
    this.pageSize = e.pageSize;
    this.loadBookings();
  }
}
