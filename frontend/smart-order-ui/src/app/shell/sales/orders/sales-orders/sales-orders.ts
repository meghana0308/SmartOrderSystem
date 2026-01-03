import { Component, OnInit, ChangeDetectorRef } from '@angular/core'
import { CommonModule, DatePipe, CurrencyPipe } from '@angular/common'
import { MatTableModule } from '@angular/material/table'
import { MatDialog } from '@angular/material/dialog'
import { SalesOrderService } from '../../../../core/services/sales-order-service'
import { OrderListDto } from '../../../../models/sales-order.model'
import { EditOrderDialogComponent } from '../sales-edit-order/sales-edit-order'

@Component({
  standalone: true,
  selector: 'app-sales-orders',
  imports: [CommonModule, MatTableModule, DatePipe, CurrencyPipe],
  templateUrl: './sales-orders.html'
})
export class SalesOrdersComponent implements OnInit {

  displayedColumns = ['orderId', 'orderDate', 'status', 'totalAmount', 'actions']
  orders: OrderListDto[] = []
  dataLoaded = false

  constructor(
    private service: SalesOrderService,
    private cdr: ChangeDetectorRef,
    private dialog: MatDialog
  ) {}

  ngOnInit() {
    this.loadOrders()
  }

  loadOrders() {
    this.service.getCreatedOrders().subscribe(res => {
      this.orders = res
      this.dataLoaded = true
      this.cdr.detectChanges()
    })
  }

  editOrder(order: OrderListDto) {
    const ref = this.dialog.open(EditOrderDialogComponent, { data: { order } })
    ref.afterClosed().subscribe(updated => {
      if (updated) this.loadOrders()
    })
  }

  cancelOrder(orderId: number) {
    if (!confirm('Are you sure you want to cancel this order?')) return
    this.service.cancelOrder(orderId).subscribe({
      next: () => this.loadOrders(),
      error: err => alert('Cancel failed: ' + JSON.stringify(err))
    })
  }
}
