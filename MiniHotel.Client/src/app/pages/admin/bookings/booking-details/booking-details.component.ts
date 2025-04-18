import { Component, OnInit } from '@angular/core';
import { Location } from '@angular/common';
import {
  BookingDto,
  BookingStatus,
  InvoiceDto,
  InvoiceItemCreateDto,
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
import { catchError, EMPTY, finalize, Observable, of, tap } from 'rxjs';
import { DialogService } from '../../../../shared/services/dialog.service';
import { AddInvoiceItemDialogComponent } from './add-invoice-item-dialog/add-invoice-item-dialog.component';

@Component({
  selector: 'app-booking-details',
  standalone: false,
  templateUrl: './booking-details.component.html',
  styleUrl: './booking-details.component.css',
})
export class BookingDetailsComponent implements OnInit {
  InvoiceStatus = InvoiceStatus;
  booking!: BookingDto;
  invoice!: InvoiceDto;
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
    private dialogService: DialogService,
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
          this.invoice.status = InvoiceStatus.Paid;
          this.loadInvoice();
        },
        error: () => this.toastr.error('Не вдалося провести офлайн оплату'),
      });
  }

  back(): void {
    this.location.back();
  }

  checkIn(): void {
    this.executeAction(
      this.bookingsService.checkInBooking({ id: this.bookingId }),
      'Гість заселений',
      'Не вдалося заселити гостя'
    );
  }

  checkOut(): void {
    this.executeAction(
      this.bookingsService.checkOutBooking({ id: this.bookingId }),
      'Бронювання закрито',
      'Не вдалося виконати check-out'
    );
  }

  cancel(): void {
    this.executeAction(
      this.bookingsService.cancelBooking({ id: this.bookingId }),
      'Бронювання скасовано',
      'Не вдалося скасувати бронювання'
    );
  }

  confirm(): void {
    this.executeAction(
      this.bookingsService.confirmedBooking({ id: this.bookingId }),
      'Бронювання підтверджено',
      'Не вдалося підтвердити бронювання'
    );
  }

  openAddServiceDialog(): void {
    this.dialogService
      .openEntityDialog<undefined, InvoiceItemCreateDto>(
        AddInvoiceItemDialogComponent,
        undefined,
        (invoiceItem) =>
          this.invoicesService.addInvoiceItem({
            invoiceId: this.invoice.invoiceId!,
            body: invoiceItem,
          }),
        'Послугу додано',
        '500px'
      )
      .subscribe(() => this.loadInvoice());
  }

  removeItem(itemId: number): void {
    this.invoicesService
      .removeInvoiceItem({ id: itemId })
      .pipe(tap(() => this.toastr.success('Послугу видалено')))
      .subscribe({
        next: () => this.loadInvoice(),
        error: (err) => {
          this.toastr.error('Не вдалося видалити послугу');
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

  private executeAction(
    action$: Observable<any>,
    successMsg: string,
    errorMsg: string
  ): void {
    action$
      .pipe(
        tap(() => {
          this.toastr.success(successMsg);
          this.loadData();
        }),
        catchError((err) => {
          this.toastr.error(errorMsg);
          console.log(err);
          return EMPTY;
        })
      )
      .subscribe();
  }

  get permissions() {
    const status = this.booking.bookingStatus;
    const invoiceStatus = this.invoice.status;

    const isPaid = invoiceStatus === InvoiceStatus.Paid;
    const isNotPaid = invoiceStatus === InvoiceStatus.Pending || invoiceStatus === InvoiceStatus.PartiallyPaid;

    return {
      canConfirm: status === BookingStatus.Pending,
      canCheckIn: status === BookingStatus.Confirmed && isPaid,
      canCheckOut: status === BookingStatus.CheckedIn && isPaid,
      canCancel:
        status === BookingStatus.Pending || status === BookingStatus.Confirmed,
      canAddService: status === BookingStatus.CheckedIn,
      canPayOffline: isNotPaid,
      canEdit:
        status === BookingStatus.Pending || status === BookingStatus.Confirmed,
    };
  }
}
