import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { InvoiceItemCreateDto, ServiceDto } from '../../../../../api/models';
import { MatDialogRef } from '@angular/material/dialog';
import { ServicesService } from '../../../../../api/services';

@Component({
  selector: 'app-add-invoice-item-dialog',
  standalone: false,
  templateUrl: './add-invoice-item-dialog.component.html',
  styleUrl: './add-invoice-item-dialog.component.css'
})
export class AddInvoiceItemDialogComponent implements OnInit {
  form: FormGroup;
  services: ServiceDto[] = [];

  constructor(
    public dialogRef: MatDialogRef<AddInvoiceItemDialogComponent>,
    private hotelServices: ServicesService,
    private fb: FormBuilder,
  ) {
    this.form = this.fb.group({
      serviceName: ['', Validators.required],
      description: ['', Validators.maxLength(200)],
      quantity: [1, [Validators.required, Validators.min(1)]]
    })
  }

  ngOnInit(): void {
    this.hotelServices.getServices()
      .subscribe(s => this.services = s)
  }

  submit(): void {
    if (this.form.valid) {
      this.dialogRef.close(this.form.value as InvoiceItemCreateDto);
    }
  }

  cancel(): void {
    this.dialogRef.close(false);
  }
}
