import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { FinanceService } from '../../../core/services/finance-service';
import { FinanceOrderDto } from '../../../models/invoice.model';
import { InvoiceDetailsPopupComponent } from '../invoices/invoice-details-popup';

@Component({
  selector: 'app-finance-orders',
  standalone: true,
  imports: [CommonModule, MatTableModule, MatButtonModule, MatDialogModule],
  templateUrl: './finance-orders.html',
})
export class FinanceOrdersComponent implements OnInit {
  private financeService = inject(FinanceService);
  private dialog = inject(MatDialog);
  private cd = inject(ChangeDetectorRef);

  orders: FinanceOrderDto[] = [];
  displayedColumns = [
    'orderId',
    'orderDate',
    'orderStatus',
    'paymentMode',
    'paymentStatus',
    'totalAmount',
    'invoice'
  ];

  ngOnInit() {
    this.loadOrders();
  }

  loadOrders() {
    this.financeService.getFinanceOrders().subscribe((res) => {
      this.orders = res;
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
