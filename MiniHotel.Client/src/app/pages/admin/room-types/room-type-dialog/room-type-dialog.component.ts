import { Component, Inject } from '@angular/core';
import { RoomTypeUpsertDto } from '../../../../api/models';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';

export interface RoomTypeFormData {
  isEdit: boolean;
  formData?: RoomTypeUpsertDto;
}

@Component({
  selector: 'app-room-type-dialog',
  standalone: false,
  templateUrl: './room-type-dialog.component.html',
  styleUrl: './room-type-dialog.component.css'
})
export class RoomTypeDialogComponent {
  form: FormGroup;

  constructor(
    public dialogRef: MatDialogRef<RoomTypeDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: RoomTypeFormData,
    private fb: FormBuilder
  ) {
    this.form = this.fb.group({
      roomCategory: [data.formData?.roomCategory || '', Validators.required],
      pricePerNight: [data.formData?.pricePerNight || '', [Validators.required, Validators.min(0)]],
      description: [data.formData?.description || '', Validators.maxLength(500)],
    });
  }

  onSubmit(): void {
    if (this.form.valid) {
      this.dialogRef.close(this.form.value as RoomTypeUpsertDto);
    }
  }

  onCancel(): void {
    this.dialogRef.close();
  }
}
