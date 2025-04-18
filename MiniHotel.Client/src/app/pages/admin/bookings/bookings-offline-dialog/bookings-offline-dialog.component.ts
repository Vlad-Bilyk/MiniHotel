import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { BookingCreateByReceptionDto, PaymentMethod, RoomDto } from '../../../../api/models';
import { MatDialogRef } from '@angular/material/dialog';
import { dateRangeValidator } from '../../../../shared/validators/date-range.validator';
import { finalize, Subject, takeUntil } from 'rxjs';
import { RoomsService } from '../../../../api/services';

@Component({
  selector: 'app-bookings-offline-dialog',
  standalone: false,
  templateUrl: './bookings-offline-dialog.component.html',
  styleUrl: './bookings-offline-dialog.component.css',
})
export class BookingsOfflineDialogComponent implements OnInit {
  form!: FormGroup;
  paymentMethods = Object.values(PaymentMethod);
  availableRooms: RoomDto[] = []
  loadingRooms = false;

  private destroy$ = new Subject<void>();

  constructor(
    private fb: FormBuilder,
    private roomsService: RoomsService,
    private dialogRef: MatDialogRef<BookingsOfflineDialogComponent>
  ) { }

  ngOnInit(): void {
    this.form = this.fb.group(
      {
        roomNumber: ['', Validators.required],
        startDate: [null, Validators.required],
        endDate: [null, Validators.required],
        fullName: ['', [Validators.required, Validators.maxLength(100)]],
        phoneNumber: [
          '',
          [Validators.required, Validators.pattern(/^\+?\d{7,15}$/)],
        ],
        paymentMethod: [PaymentMethod.OnSite, [Validators.required]],
      },
      {
        validators: dateRangeValidator,
      }
    );
  }

  submit(): void {
    if (this.form.valid) {
      this.dialogRef.close(this.form.value as BookingCreateByReceptionDto);
    }
  }

  cancel(): void {
    this.dialogRef.close(false);
  }

  onDatesChange(): void {
    const { startDate, endDate } = this.form.value;
    if (this.form.get('startDate')!.valid && this.form.get('endDate')!.valid) {
      this.loadAvailableRooms(startDate, endDate);
    }
  }

  loadAvailableRooms(start: Date, end: Date): void {
    this.loadingRooms = true;
    this.roomsService.getAvailableRooms({ startDate: start.toDateString(), endDate: end.toDateString() })
      .pipe(finalize(() => this.loadingRooms = false),
        takeUntil(this.destroy$)
      )
      .subscribe(list => {
        this.availableRooms = list;
        const selected = this.form.get('roomNumber')!.value;
        if (selected && !list.some(r => r.roomNumber === selected)) {
          this.form.get('roomNumber')!.setValue('');
        }
      });
  }
}
