import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { UsersService } from '../../../../api/services';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { UserUpdateDto } from '../../../../api/models';

export interface UserFormData {
  formData?: UserUpdateDto;
}

@Component({
  selector: 'app-users-update-dialog',
  standalone: false,
  templateUrl: './users-update-dialog.component.html',
  styleUrl: './users-update-dialog.component.scss',
})
export class UsersUpdateDialogComponent implements OnInit {
  form!: FormGroup;

  constructor(
    public dialogRef: MatDialogRef<UsersUpdateDialogComponent>,
    private usersService: UsersService,
    @Inject(MAT_DIALOG_DATA) public data: UserFormData,
    private fb: FormBuilder
  ) {
    this.form = this.fb.group({
      firstName: [data.formData?.firstName || '', [Validators.required]],
      lastName: [data.formData?.lastName || '', [Validators.required]],
      email: [
        data.formData?.email || '',
        [Validators.required, Validators.email],
      ],
      phoneNumber: [
        data.formData?.phoneNumber || '',
        [Validators.required, Validators.pattern(/^\+?\d{7,15}$/)],
      ],
    });
  }

  ngOnInit(): void { }

  onSubmit(): void {
    if (this.form.valid) {
      this.dialogRef.close(this.form.value as UserUpdateDto);
    }
  }

  onCancel(): void {
    this.dialogRef.close();
  }
}
