import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { BookingsService } from '../../api/services';
import { BookingCreateDto, BookingDto } from '../../api/models';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-booking-confirmation',
  standalone: false,
  templateUrl: './booking-confirmation.component.html',
  styleUrl: './booking-confirmation.component.css',
})
export class BookingConfirmationComponent implements OnInit {
  bookingData: BookingCreateDto | null = null;
  isLoading = false;

  totalPrice = 0;
  pricePerDay = 0;
  nights = 0;
  roomType: string = '';

  paymentForm!: FormGroup;

  constructor(
    private router: Router,
    private toastr: ToastrService,
    private bookingsService: BookingsService,
    private fb: FormBuilder
  ) { }

  ngOnInit(): void {
    const state = JSON.parse(localStorage.getItem('bookingData') || 'null');

    if (
      !state ||
      !state.roomNumber ||
      !state.startDate ||
      !state.endDate ||
      !state.pricePerDay
    ) {
      this.toastr.error('Недостатньо даних для підтвердження бронювання.');
      this.router.navigate(['/']);
      return;
    }

    this.bookingData = {
      roomNumber: state.roomNumber,
      startDate: new Date(state.startDate).toISOString(),
      endDate: new Date(state.endDate).toISOString(),
    };

    this.pricePerDay = state.pricePerDay;
    const start = new Date(state.startDate);
    const end = new Date(state.endDate);
    this.nights = Math.ceil(
      (end.getTime() - start.getTime()) / (1000 * 60 * 60 * 24)
    );
    this.totalPrice = this.nights * this.pricePerDay;
    this.roomType = state.roomType;

    this.paymentForm = this.fb.group({
      selectedPayment: ['online', Validators.required],
      comment: [''],
    });
  }

  confirmBooking(): void {
    if (!this.bookingData) return;

    const paymentMethod = this.paymentForm.get('selectedPayment')?.value;
    const comment = this.paymentForm.get('comment')?.value;

    this.isLoading = true;

    this.bookingsService.createBooking({ body: this.bookingData }).subscribe({
      next: (booking: BookingDto) => {
        this.toastr.success('Бронювання успішно створено!');
        if (paymentMethod === 'online' || paymentMethod === 'partial') {
          this.router.navigate(['/payments', booking.bookingId]);
        } else {
          this.router.navigate(['/']);
          this.toastr.info('Очікуємо оплату на місці. Дякуємо за бронювання!');
        }

        console.log('User comment:', comment);
      },
      error: () => {
        this.toastr.error('Не вдалося створити бронювання.');
      },
      complete: () => (this.isLoading = false),
    });
  }
}
