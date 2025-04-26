import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { BookingsService, PaymentsService } from '../../api/services';
import { BookingCreateDto, BookingDto, PaymentMethod } from '../../api/models';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { catchError, of, finalize } from 'rxjs';

interface StoredBookingData {
  roomNumber: string;
  startDate: string;
  endDate: string;
  pricePerDay: number;
  roomType?: string;
}

@Component({
  selector: 'app-booking-confirmation',
  standalone: false,
  templateUrl: './booking-confirmation.component.html',
  styleUrl: './booking-confirmation.component.css',
})
export class BookingConfirmationComponent implements OnInit {
  paymentForm!: FormGroup;
  isLoading = false;

  totalPrice = 0;
  nights = 0;

  bookingData!: StoredBookingData;

  constructor(
    private router: Router,
    private toastr: ToastrService,
    private bookingsService: BookingsService,
    private paymentsService: PaymentsService,
    private fb: FormBuilder
  ) { }

  // Map radio selection to enum
  private paymentMap: Record<string, PaymentMethod> = {
    online: PaymentMethod.Online,
    onsite: PaymentMethod.OnSite,
  };

  ngOnInit(): void {
    this.loadStoredData();
    this.initForm();
  }

  confirmBooking(): void {
    if (this.paymentForm.invalid) {
      this.paymentForm.markAllAsTouched();
      return;
    }

    const dto = this.buildDto();
    this.isLoading = true;

    this.bookingsService
      .createBooking({ body: dto })
      .pipe(
        catchError((err) => {
          console.error(err);
          this.toastr.error('Не вдалося створити бронювання.');
          return of(null);
        }),
        finalize(() => (this.isLoading = false))
      )
      .subscribe((booking: BookingDto | null) => {
        if (!booking) {
          return;
        }
        this.handleSuccess(booking);
      });
  }

  private loadStoredData(): void {
    const raw = localStorage.getItem('bookingData');
    if (!raw) {
      this.abort('Недостатньо даних для підтвердження бронювання.');
      return;
    }

    const state = JSON.parse(raw) as StoredBookingData;
    if (
      !state.roomNumber ||
      !state.startDate ||
      !state.endDate ||
      !state.pricePerDay
    ) {
      this.abort('Недостатньо даних для підтвердження бронювання.');
      return;
    }

    this.bookingData = state;
    const start = new Date(state.startDate);
    const end = new Date(state.endDate);
    this.nights = Math.ceil(
      (end.getTime() - start.getTime()) / (1000 * 60 * 60 * 24)
    );
    this.totalPrice = this.nights * state.pricePerDay;
  }

  private abort(message: string): void {
    this.toastr.error(message);
    this.router.navigate(['/']);
  }

  private initForm(): void {
    this.paymentForm = this.fb.group({
      selectedPayment: ['online', Validators.required],
      comment: [''],
    });
  }

  private buildDto(): BookingCreateDto {
    const { selectedPayment, comment } = this.paymentForm.value;
    return {
      roomNumber: this.bookingData.roomNumber,
      startDate: this.bookingData.startDate,
      endDate: this.bookingData.endDate,
      paymentMethod: this.paymentMap[selectedPayment],
    };
  }

  private handleSuccess(booking: BookingDto): void {
    this.toastr.success('Бронювання успішно створено!');
    const method = this.paymentForm.value.selectedPayment;
    if (method === 'online' || method === 'partial') {
      this.redirectToPayment(booking.invoiceId!);
    } else {
      this.toastr.info('Очікуємо оплату при заселенні.');
      this.router.navigate(['/my-bookings']);
    }
  }

  private redirectToPayment(invoiceId: number): void {
    this.paymentsService.payOnline$Plain({ invoiceId }).subscribe({
      next: (html) => {
        document.body.innerHTML = html;
        (document.querySelector('form') as HTMLFormElement).submit();
      },
      error: (err) => {
        console.error(err);
        this.toastr.error('Не вдалося розпочати оплату онлайн.');
        this.router.navigate(['/my-bookings']);
      },
    });
  }
}
