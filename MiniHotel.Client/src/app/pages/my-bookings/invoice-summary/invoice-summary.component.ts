import { Component, Inject, Input } from '@angular/core';
import { InvoiceDto, InvoiceStatus } from '../../../api/models';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { StatusStyleService } from '../../../shared/services/status-style.service';

@Component({
  selector: 'app-invoice-summary',
  standalone: false,
  templateUrl: './invoice-summary.component.html',
  styleUrl: './invoice-summary.component.scss'
})
export class InvoiceSummaryComponent {
  @Input() invoice!: InvoiceDto;

  constructor(
    private statusStyleService: StatusStyleService,
    @Inject(MAT_DIALOG_DATA) public data?: { invoice: InvoiceDto }) {
    if (data?.invoice) {
      this.invoice = data.invoice;
    }
  }

  getInvoiceChipClass(status: InvoiceStatus | string): string {
    return this.statusStyleService.getInvoiceStatusClass(status);
  }
}
