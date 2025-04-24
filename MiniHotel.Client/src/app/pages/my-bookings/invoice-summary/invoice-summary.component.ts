import { Component, Inject, Input } from '@angular/core';
import { InvoiceDto, InvoiceStatus } from '../../../api/models';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'app-invoice-summary',
  standalone: false,
  templateUrl: './invoice-summary.component.html',
  styleUrl: './invoice-summary.component.scss'
})
export class InvoiceSummaryComponent {
  @Input() invoice!: InvoiceDto;

  constructor(@Inject(MAT_DIALOG_DATA) public data?: { invoice: InvoiceDto }) {
    if (data?.invoice) {
      this.invoice = data.invoice;
    }
  }

  statusClass(status: string): string {
    switch (status) {
      case InvoiceStatus.Pending: return 'badge-warning';
      case InvoiceStatus.Paid: return 'badge-success';
      case InvoiceStatus.Cancelled: return 'badge-danger';
      default: return 'badge-secondary';
    }
  }
}
