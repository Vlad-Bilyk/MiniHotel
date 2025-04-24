import { Component, OnInit, ViewChild } from '@angular/core';
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
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { BookingsOfflineDialogComponent } from './bookings-offline-dialog/bookings-offline-dialog.component';
import { DialogService } from '../../../shared/services/dialog.service';
import { formatDate } from '@angular/common';
import { BookingEditFormData, EditBookingDialogComponent } from './edit-booking-dialog/edit-booking-dialog.component';

const STATUS_ORDER: Record<BookingStatus, number> = {
  [BookingStatus.Pending]: 1,
  [BookingStatus.Confirmed]: 2,
  [BookingStatus.CheckedIn]: 3,
  [BookingStatus.CheckedOut]: 4,
  [BookingStatus.Cancelled]: 5,
};

@Component({
  selector: 'app-bookings',
  standalone: false,
  templateUrl: './bookings.component.html',
  styleUrl: './bookings.component.scss',
})
export class BookingsComponent implements OnInit {
  displayedColumns = ['client', 'room', 'roomType', 'dates', 'payment', 'status', 'actions'];

  BookingStatus = BookingStatus;
  PaymentMethod = PaymentMethod;
  dataSource = new MatTableDataSource<BookingDto>([]);
  searchTerm = '';
  loading = true;

  @ViewChild(MatPaginator) set MatPaginator(p: MatPaginator) {
    this.dataSource.paginator = p;
  }
  @ViewChild(MatSort) set MatSort(s: MatSort) {
    this.dataSource.sort = s;
  }

  constructor(
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
    this.bookingsService.getBookings().subscribe({
      next: (bookings) => {
        this.dataSource = new MatTableDataSource(bookings);

        this.configureSorting();
        this.configureFiltering();

        this.loading = false;
      },
      error: (err) => {
        this.toastr.error('Не вдалося завантажити бронювання');
        console.error(err);
        this.loading = false;
      },
    });
  }

  applyFilter(): void {
    this.dataSource.filter = this.searchTerm.trim().toLowerCase();
  }

  viewDetails(id: number): void {
    this.router.navigate(['admin/booking-details', id]);
  }

  editBooking(booking: BookingDto): void {
    this.dialogService.openEntityDialog<BookingEditFormData, BookingUpdateDto>(
      EditBookingDialogComponent,
      {
        formData: {
          roomNumber: booking.roomNumber!,
          startDate: booking.startDate!,
          endDate: booking.endDate!,
        }
      },
      (data) => this.bookingsService.updateBooking({ id: booking.bookingId!, body: data }),
      'Бронювання оновлено'
    ).subscribe(() => {
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

  private configureSorting(): void {
    this.dataSource.sortingDataAccessor = (item, property) => {
      switch (property) {
        case 'startDate':
          return item.startDate ? new Date(item.startDate as any) : new Date(0);
        case 'status':
          return STATUS_ORDER[item.bookingStatus!] ?? Number.MAX_SAFE_INTEGER;
        default:
          return (item as any)[property];
      }
    };
  }

  private configureFiltering(): void {
    this.dataSource.filterPredicate = (data, filter) => {
      const search = filter.trim().toLowerCase();

      const start = data.startDate
        ? formatDate(data.startDate, 'dd.MM.yyyy', "uk-UA").toLowerCase()
        : '';

      const end = data.endDate
        ? formatDate(data.endDate, 'dd.MM.yyyy', 'uk-UA').toLowerCase()
        : '';

      return Boolean(
        (data.fullName?.toLowerCase().includes(search) ?? false) ||
        (data.roomNumber?.toLowerCase().includes(search) ?? false) ||
        (data.roomCategory?.toLowerCase().includes(search) ?? false) ||
        (data.paymentMethod?.toLowerCase().includes(search) ?? false) ||
        (data.paymentMethod?.toLowerCase().includes(search) ?? false) ||
        (data.bookingStatus?.toLowerCase().includes(search) ?? false) ||
        start.includes(search) ||
        end.includes(search)
      );
    };
  }
}
