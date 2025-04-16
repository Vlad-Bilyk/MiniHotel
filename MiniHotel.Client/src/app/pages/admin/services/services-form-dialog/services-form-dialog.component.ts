import { Component, Inject } from '@angular/core';
import { ServiceUpsertDto } from '../../../../api/models';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

export interface ServiceFormData {
  isEdit: boolean;
  formData?: ServiceUpsertDto;
}

@Component({
  selector: 'app-services-form-dialog',
  standalone: false,
  templateUrl: './services-form-dialog.component.html',
  styleUrl: './services-form-dialog.component.css'
})
export class ServicesFormDialogComponent {
  form: FormGroup;

  constructor(
    public dialogRef: MatDialogRef<ServicesFormDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: ServiceFormData,
    private fb: FormBuilder
  ) {
    this.form = this.fb.group({
      name: [data.formData?.name ?? '', [Validators.required, Validators.maxLength(100)]],
      description: [data.formData?.description ?? '', [Validators.required, Validators.maxLength(500)]],
      price: [data.formData?.price ?? 0, [Validators.required, Validators.min(0)]],
      isAvailable: [data.formData?.isAvailable ?? true]
    });
  }

  onSubmit(): void {
    if (this.form.valid) {
      this.dialogRef.close(this.form.value as ServiceUpsertDto);
    }
  }

  onCancel(): void {
    this.dialogRef.close();
  }
}
