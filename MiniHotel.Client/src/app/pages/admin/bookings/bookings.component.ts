import { Component, OnInit, ViewChild } from '@angular/core';
import {
  BookingCreateByReceptionDto,
  BookingDto,
  BookingStatus,
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
  styleUrl: './bookings.component.css',
})
export class BookingsComponent implements OnInit {
  BookingStatus = BookingStatus;
  PaymentMethod = PaymentMethod;
  displayedColumns = [
    'client',
    'roomNumber',
    'roomType',
    'dates',
    'startDate',
    'payment',
    'status',
    'actions',
  ];
  dataSource = new MatTableDataSource<BookingDto>();
  searchTerm = '';

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

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
    this.bookingsService.getBookings().subscribe({
      next: (bookings) => {
        this.dataSource = new MatTableDataSource(bookings);
        this.dataSource.paginator = this.paginator;
        this.dataSource.sort = this.sort;

        this.configureSorting();
        this.configureFiltering();

        this.sort.sort({
          id: 'status',
          start: 'asc',
          disableClear: false
        });
      },
      error: (err) => {
        this.toastr.error('Не вдалося завантажити бронювання');
        console.error(err);
      },
    });
  }

  applyFilter(): void {
    this.dataSource.filter = this.searchTerm.trim().toLowerCase();
  }

  viewDetails(id: number): void {
    this.router.navigate(['admin/booking-details', id]);
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

      return Boolean(
        (data.fullName?.toLowerCase().includes(search) ?? false) ||
        (data.roomNumber?.toLowerCase().includes(search) ?? false) ||
        (data.roomCategory?.toLowerCase().includes(search) ?? false) ||
        (data.paymentMethod?.toLowerCase().includes(search) ?? false) ||
        (data.bookingStatus?.toLowerCase().includes(search) ?? false) ||
        data.startDate?.toString().toLowerCase().includes(search) ||
        data.endDate?.toString().toLowerCase().includes(search)
      );
    };
  }
}
