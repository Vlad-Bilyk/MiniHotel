import { Component, OnInit, ViewChild } from '@angular/core';
import { BookingDto, BookingStatus } from '../../../api/models';
import { BookingsService } from '../../../api/services';
import { ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { ThemePalette } from '@angular/material/core';
import { formatDate } from '@angular/common';

@Component({
  selector: 'app-bookings',
  standalone: false,
  templateUrl: './bookings.component.html',
  styleUrl: './bookings.component.css',
})
export class BookingsComponent implements OnInit {
  BookingStatus = BookingStatus;
  displayedColumns = [
    'client',
    'room',
    'roomType',
    'dates',
    'startDate',
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
    private dialog: MatDialog
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

  checkIn(id: number): void {
    this.bookingsService.checkInBooking({ id }).subscribe({
      next: () => {
        this.toastr.success('Гість заселений');
        this.loadBookings();
      },
      error: (err) => {
        this.toastr.error('Не вдалося заселити гостя');
        console.error(err);
      },
    });
  }

  checkOut(id: number): void {
    this.bookingsService.checkOutBooking({ id }).subscribe({
      next: () => {
        this.toastr.success('Бронювання закрито');
        this.loadBookings();
      },
      error: (err) => {
        this.toastr.error('Не вдалося виконати check-out');
        console.error(err);
      },
    });
  }

  cancel(id: number): void {
    this.bookingsService.cancelBooking({ id }).subscribe({
      next: () => {
        this.toastr.success('Бронювання скасовано');
        this.loadBookings();
      },
      error: (err) => {
        this.toastr.error('Не вдалося скасувати бронювання');
        console.error(err);
      },
    });
  }

  confirm(id: number): void {
    this.bookingsService.confirmedBooking({ id }).subscribe({
      next: () => {
        this.toastr.success('Бронювання підтверджено');
        this.loadBookings();
      },
      error: (err) => {
        this.toastr.error('Не вдалося підтвердити бронювання');
        console.error(err);
      },
    });
  }

  // TODO: add real navigate
  viewDetails(bookingId: number): void {
    this.router.navigate(['/']);
  }

  // TODO: open dialog with create form
  createOfflineBooking(): void { }

  private configureSorting(): void {
    this.dataSource.sortingDataAccessor = (item, property) => {
      switch (property) {
        case 'startDate':
          return item.startDate ? new Date(item.startDate as any) : new Date(0);
        default:
          return (item as any)[property];
      }
    };
  }

  private configureFiltering(): void {
    this.dataSource.filterPredicate = (data, filter) => {
      const search = filter.trim().toLowerCase();

      const formattedStart = data.startDate
        ? formatDate(data.startDate as any, 'dd.MM.yyyy', 'en-US')
        : '';

      const formattedEnd = data.endDate
        ? formatDate(data.endDate as any, 'dd.MM.yyyy', 'en-US')
        : '';

      return Boolean(
        (data.fullName?.toLowerCase().includes(search) ?? false) ||
        (data.roomNumber?.toLowerCase().includes(search) ?? false) ||
        (data.roomCategory?.toLowerCase().includes(search) ?? false) ||
        (data.bookingStatus?.toLowerCase().includes(search) ?? false) ||
        formattedStart.toString().toLowerCase().includes(search) ||
        formattedEnd.toString().toLowerCase().includes(search)
      );
    };
  }
}
