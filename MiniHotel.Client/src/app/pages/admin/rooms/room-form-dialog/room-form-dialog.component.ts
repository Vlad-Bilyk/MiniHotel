import { Component, Inject, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { RoomTypeDto, RoomStatus, RoomUpsertDto } from '../../../../api/models';
import { RoomTypesService } from '../../../../api/services';

export interface RoomFormData {
  isEdit: boolean;
  formData?: RoomUpsertDto;
}

@Component({
  selector: 'app-room-form-dialog',
  standalone: false,
  templateUrl: './room-form-dialog.component.html',
  styleUrl: './room-form-dialog.component.css'
})
export class RoomFormDialogComponent implements OnInit {
  form: FormGroup;
  roomTypes: RoomTypeDto[] = [];
  roomStatuses = Object.values(RoomStatus);

  constructor(
    public dialogRef: MatDialogRef<RoomFormDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: RoomFormData,
    private fb: FormBuilder,
    private roomTypeService: RoomTypesService
  ) {
    this.form = this.fb.group({
      roomNumber: [data.formData?.roomNumber || '', [Validators.required, Validators.maxLength(10)]],
      roomTypeId: [data.formData?.roomTypeId || null, Validators.required],
      roomStatus: [data.formData?.roomStatus || RoomStatus.Available, Validators.required],
    });
  }

  ngOnInit(): void {
    this.roomTypeService.getRoomTypes().subscribe({
      next: (types) => this.roomTypes = types
    });
  }

  onSubmit(): void {
    if (this.form.valid) {
      this.dialogRef.close(this.form.value as RoomUpsertDto);
    }
  }

  onCancel(): void {
    this.dialogRef.close();
  }
}
