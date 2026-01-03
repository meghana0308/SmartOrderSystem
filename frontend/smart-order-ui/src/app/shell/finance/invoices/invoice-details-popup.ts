import { Component, inject, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { CommonModule } from '@angular/common';
import { FinanceService } from '../../../core/services/finance-service';
import { InvoiceDetailDto } from '../../../models/invoice.model';
import { ChangeDetectorRef } from '@angular/core';

@Component({
  selector: 'app-invoice-details-popup',
  standalone: true,
  imports: [CommonModule, MatDialogModule],
  templateUrl: './invoice-details-popup.html'
})
export class InvoiceDetailsPopupComponent {
  private financeService = inject(FinanceService);
  private cd = inject(ChangeDetectorRef);

  invoice: InvoiceDetailDto | null = null;

  constructor(@Inject(MAT_DIALOG_DATA) private data: { invoiceId: number }) {
    this.loadInvoice(data.invoiceId);
  }

  loadInvoice(id: number) {
    this.financeService.getInvoiceById(id).subscribe((res) => {
      this.invoice = res;
      this.cd.detectChanges(); // update view safely
    });
  }
}
