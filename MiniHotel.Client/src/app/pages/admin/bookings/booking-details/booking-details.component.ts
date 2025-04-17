import { Component, OnInit } from '@angular/core';
import { Location } from '@angular/common';
import {
  BookingDto,
  BookingStatus,
  InvoiceDto,
  InvoiceItemDto,
  InvoiceStatus,
  PaymentMethod,
} from '../../../../api/models';
import { ActivatedRoute, Router } from '@angular/router';
import {
  BookingsService,
  InvoicesService,
  PaymentsService,
} from '../../../../api/services';
import { ToastrService } from 'ngx-toastr';
import { catchError, finalize, of } from 'rxjs';

@Component({
  selector: 'app-booking-details',
  standalone: false,
  templateUrl: './booking-details.component.html',
  styleUrl: './booking-details.component.css',
})
export class BookingDetailsComponent implements OnInit {
  InvoiceStatus = InvoiceStatus;
  booking!: BookingDto;
  invoice?: InvoiceDto;
  items: InvoiceItemDto[] = [];
  paymentMethod = PaymentMethod;
  loading = true;
  payLoading = false;

  bookingLoading = false;
  BookingStatus = BookingStatus;
  bookingId!: number;

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private bookingsService: BookingsService,
    private invoicesService: InvoicesService,
    private paymentsService: PaymentsService,
    private toastr: ToastrService,
    private location: Location
  ) { }

  ngOnInit(): void {
    this.bookingId = Number(this.route.snapshot.paramMap.get('id'));
    this.loadData();
  }

  payOffline(): void {
    if (!this.invoice) {
      return;
    }
    this.payLoading = true;
    this.paymentsService
      .payOffline({ invoiceId: this.invoice.invoiceId! })
      .pipe(finalize(() => (this.payLoading = false)))
      .subscribe({
        next: () => {
          this.toastr.success('Оплата по рахунку проведена офлайн');
          this.invoice!.status = InvoiceStatus.Paid;
        },
        error: () => this.toastr.error('Не вдалося провести офлайн оплату'),
      });
  }

  back(): void {
    this.location.back();
  }

  checkIn(id: number): void {
    this.bookingsService.checkInBooking({ id }).subscribe({
      next: () => {
        this.toastr.success('Гість заселений');
        this.loadData();
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
        this.loadData();
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
        this.loadData();
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
        this.loadData();
      },
      error: (err) => {
        this.toastr.error('Не вдалося підтвердити бронювання');
        console.error(err);
      },
    });
  }

  private loadData(): void {
    this.loadInvoice();
    this.loadBooking();
  }

  private loadInvoice(): void {
    this.invoicesService
      .getInvoiceByBookingId({ bookingId: this.bookingId })
      .pipe(
        catchError((err) => {
          console.error(err);
          this.toastr.error('Не вдалося завантажити рахунок');
          return of(null);
        }),
        finalize(() => (this.loading = false))
      )
      .subscribe((inv) => {
        if (inv) {
          this.invoice = inv;
          this.items = inv.invoiceItems ?? [];
        }
      });
  }

  private loadBooking(): void {
    this.bookingsService
      .getBookingById({ id: this.bookingId })
      .pipe(
        catchError((err) => {
          this.toastr.error('Не вдалося завантажити бронювання');
          console.error(err);
          this.router.navigate(['/bookings']);
          return of(null);
        })
      )
      .subscribe((b) => {
        if (b) {
          this.booking = b;
        }
      });
  }
}
