import { Component, Inject } from '@angular/core';
import { BookingUpdateDto, RoomDto } from '../../../../api/models';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { RoomsService } from '../../../../api/services';
import { finalize, Subject, takeUntil } from 'rxjs';
import { dateRangeValidator } from '../../../../shared/validators/date-range.validator';

export interface BookingEditFormData {
  formData?: BookingUpdateDto;
}

@Component({
  selector: 'app-edit-booking-dialog',
  standalone: false,
  templateUrl: './edit-booking-dialog.component.html',
  styleUrl: './edit-booking-dialog.component.scss',
})
export class EditBookingDialogComponent {
  form!: FormGroup;
  availableRooms: RoomDto[] = [];
  loadingRooms = false;

  private destroy$ = new Subject<void>();

  constructor(
    private roomsService: RoomsService,
    public dialogRef: MatDialogRef<EditBookingDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: BookingEditFormData,
    private fb: FormBuilder
  ) {
    const initialData = data.formData;
    this.buildForm(initialData!);
  }

  buildForm(initialData: BookingUpdateDto): void {
    const initialRoom = initialData?.roomNumber;
    const canSelect = !this.loadingRooms && this.availableRooms.length > 0;

    const parsedStartDate = new Date(initialData?.startDate ?? '');
    const parsedEndDate = new Date(initialData?.endDate ?? '');

    this.form = this.fb.group(
      {
        roomNumber: [{
          value: initialRoom ?? '',
          disabled: !canSelect
        }, [Validators.required]],
        startDate: [parsedStartDate, [Validators.required]],
        endDate: [parsedEndDate, [Validators.required]],
      },
      {
        validators: dateRangeValidator,
      }
    );
  }

  onSubmit(): void {
    if (this.form.valid) {
      this.dialogRef.close(this.form.value as BookingUpdateDto);
    }
  }

  onCancel(): void {
    this.dialogRef.close();
  }

  onDatesChange(): void {
    const { startDate, endDate } = this.form.value;
    if (this.form.get('startDate')!.valid && this.form.get('endDate')!.valid) {
      this.loadAvailableRooms(startDate, endDate);
    }
  }

  loadAvailableRooms(start: Date, end: Date): void {
    this.loadingRooms = true;
    this.form.get('roomNumber')?.disable();

    this.roomsService
      .getAvailableRooms({
        startDate: start.toDateString(),
        endDate: end.toDateString(),
      })
      .pipe(
        finalize(() => (this.loadingRooms = false)),
        takeUntil(this.destroy$)
      )
      .subscribe((list) => {
        this.availableRooms = list;

        if (list.length > 0) {
          this.form.get('roomNumber')?.enable();
        } else {
          this.form.get('roomNumber')?.disable();
        }

        const selected = this.form.get('roomNumber')!.value;
        if (selected && !list.some((r) => r.roomNumber === selected)) {
          this.form.get('roomNumber')!.reset();
        }
      });
  }
}
