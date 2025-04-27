import { Component, OnInit } from '@angular/core';
import {
  BookingCreateByReceptionDto,
  BookingDto,
  BookingStatus,
  BookingUpdateDto,
  PaymentMethod,
} from '../../../api/models';
import { BookingsService } from '../../../api/services';
import { ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';
import { PageEvent } from '@angular/material/paginator';
import { BookingsOfflineDialogComponent } from './bookings-offline-dialog/bookings-offline-dialog.component';
import { DialogService } from '../../../shared/services/dialog.service';
import {
  BookingEditFormData,
  EditBookingDialogComponent,
} from './edit-booking-dialog/edit-booking-dialog.component';
import { StatusStyleService } from '../../../shared/services/status-style.service';

@Component({
  selector: 'app-bookings',
  standalone: false,
  templateUrl: './bookings.component.html',
  styleUrl: './bookings.component.scss',
})
export class BookingsComponent implements OnInit {
  displayedColumns = [
    'client',
    'room',
    'roomType',
    'dates',
    'payment',
    'status',
    'actions',
  ];

  BookingStatus = BookingStatus;
  PaymentMethod = PaymentMethod;
  bookings: BookingDto[] = [];

  totalCount = 0;
  pageSize = 10;
  pageIndex = 0;

  searchTerm = '';
  loading = true;

  constructor(
    private statusStyleService: StatusStyleService,
    private bookingsService: BookingsService,
    private toastr: ToastrService,
    private router: Router,
    private dialogService: DialogService
  ) { }

  ngOnInit(): void {
    this.loadBookings();
  }

  loadBookings(): void {
    this.loading = true;

    this.bookingsService
      .getBookings({
        pageNumber: this.pageIndex + 1,
        pageSize: this.pageSize,
        search: this.searchTerm.trim(),
      })
      .subscribe({
        next: (result) => {
          this.bookings = result.items ?? [];
          this.totalCount = result.totalCount!;
          this.loading = false;
        },
        error: (err) => {
          this.toastr.error('Не вдалося завантажити бронювання');
          console.error(err);
          this.loading = false;
        },
      });
  }

  pageChanged(e: PageEvent): void {
    this.pageIndex = e.pageIndex;
    this.pageSize = e.pageSize;
    this.loadBookings();
  }

  applyFilter(): void {
    this.pageIndex = 0;
    this.loadBookings();
  }

  viewDetails(id: number): void {
    this.router.navigate(['admin/booking-details', id]);
  }

  editBooking(booking: BookingDto): void {
    this.dialogService
      .openEntityDialog<BookingEditFormData, BookingUpdateDto>(
        EditBookingDialogComponent,
        {
          bookingId: booking.bookingId!,
          formData: {
            roomNumber: booking.roomNumber!,
            startDate: booking.startDate!,
            endDate: booking.endDate!,
          },
        },
        (data) =>
          this.bookingsService.updateBooking({
            id: booking.bookingId!,
            body: data,
          }),
        'Бронювання оновлено'
      )
      .subscribe(() => {
        this.loadBookings();
      });
  }

  createOfflineBooking(): void {
    this.dialogService
      .openEntityDialog<undefined, BookingCreateByReceptionDto>(
        BookingsOfflineDialogComponent,
        undefined,
        (dto) => this.bookingsService.createBookingByReception({ body: dto }),
        'Офлайн-бронювання успішно створено',
        '500px'
      )
      .subscribe(() => {
        this.loadBookings();
      });
  }

  canEdit(b: BookingDto): boolean {
    const offline = b.paymentMethod !== this.PaymentMethod.Online;
    const goodStatus =
      b.bookingStatus === this.BookingStatus.Pending ||
      b.bookingStatus === this.BookingStatus.Confirmed;
    return offline && goodStatus;
  }

  getBookingChipClass(status: BookingStatus | string): string {
    return this.statusStyleService.getBookingStatusClass(status);
  }
}
