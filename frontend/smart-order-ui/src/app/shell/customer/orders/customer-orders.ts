import { Component, OnInit, inject } from '@angular/core'
import { CommonModule, DatePipe, CurrencyPipe } from '@angular/common'
import { OrderService } from '../../../core/services/order-service'
import { OrderListDto, OrderQueryDto } from '../../../models/order.model'

@Component({
  standalone: true,
  selector: 'app-customer-orders',
  templateUrl: './customer-orders.html',
  imports: [CommonModule, DatePipe, CurrencyPipe]
})
export class CustomerOrdersComponent implements OnInit {
  orders: OrderListDto[] = []
  loading = false
  page = 1
  pageSize = 10

  private readonly orderService = inject(OrderService)

  ngOnInit() {
    this.fetchOrders()
  }

  fetchOrders() {
    this.loading = true
    const query: OrderQueryDto = { page: this.page, pageSize: this.pageSize }
    this.orderService.getMyOrders(query).subscribe({
      next: (res) => { this.orders = res; this.loading = false },
      error: (err) => { alert(err.error?.message || 'Error fetching orders'); this.loading = false }
    })
  }

  canCancel(order: OrderListDto) {
    return !['Shipped', 'Delivered', 'Cancelled'].includes(order.status)
  }

  cancelOrder(orderId: number) {
    if (!confirm('Are you sure you want to cancel this order?')) return

    this.orderService.cancelOrder(orderId).subscribe({
      next: () => {
        alert('Order cancelled successfully')
        this.fetchOrders()
      },
      error: (err) => alert(err.error?.message || 'Error cancelling order')
    })
  }
}
