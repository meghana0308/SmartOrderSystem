import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms'; 
import { FinanceService } from '../../../core/services/finance-service';
import { InvoiceListDto } from '../../../models/invoice.model';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { InvoiceDetailsPopupComponent } from './invoice-details-popup';

@Component({
  selector: 'app-finance-invoices',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatTableModule,
    MatButtonModule,
    MatDialogModule
  ],
  templateUrl: './finance-invoices.html',
})
export class FinanceInvoicesComponent implements OnInit {
  private financeService = inject(FinanceService);
  private dialog = inject(MatDialog);
  private cd = inject(ChangeDetectorRef); 

  invoices: InvoiceListDto[] = [];
  displayedColumns = ['invoiceId', 'orderId', 'invoiceDate', 'totalAmount', 'paymentStatus', 'actions'];

  ngOnInit() {
    this.loadInvoices();
  }

  loadInvoices() {
    this.financeService.getInvoices().subscribe((res) => {
      this.invoices = res;
      this.cd.detectChanges(); 
    });
  }

  viewInvoice(invoiceId: number) {
    this.dialog.open(InvoiceDetailsPopupComponent, {
      data: { invoiceId },
      width: '600px',
    });
  }
}
