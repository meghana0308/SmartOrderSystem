import { AfterViewInit, Component, ViewChild, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatSort, MatSortModule } from '@angular/material/sort';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { HttpClient } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { OrderListDto } from '../../../models/order.model';
import { OrderStatus } from '../../../core/enums/order-status';

@Component({
  selector: 'app-warehouse-orders',
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule,
    MatSortModule,
    MatPaginatorModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatIconModule,
    MatSnackBarModule,
    FormsModule
  ],
  templateUrl: './warehouse-orders-status.html',
  styleUrls: ['./warehouse-orders-status.css']
})
export class WarehouseOrdersComponent implements AfterViewInit {

  displayedColumns = ['orderId', 'orderDate', 'status', 'paymentStatus', 'totalAmount'];
  dataSource = new MatTableDataSource<OrderListDto>([]);
  statusOptions = Object.keys(OrderStatus).filter(k => Number.isNaN(Number(k))); 

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  constructor(
    private readonly http: HttpClient,
    private readonly snack: MatSnackBar,
    private readonly cd: ChangeDetectorRef
  ) {
    this.loadOrders();
  }

  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
  }

  loadOrders() {
    this.http.get<OrderListDto[]>('https://localhost:7267/api/orders/all')
      .subscribe({
        next: res => {
          setTimeout(() => {
            this.dataSource.data = res;
            this.cd.detectChanges();
          });
        },
        error: () => this.snack.open('Failed to load orders', 'Close', { duration: 3000 })
      });
  }

  applyFilter(event: any) {
    const filterValue = event.target.value.toLowerCase().trim();
    this.dataSource.filter = filterValue;
  }

  updateStatus(order: OrderListDto, newStatus: string) {
    const previousStatus = order.status;

    this.http.put(`https://localhost:7267/api/orders/${order.orderId}/status`, { status: newStatus })
      .subscribe({
        next: () => {
          // Reload only the updated order from backend
          this.http.get<OrderListDto[]>('https://localhost:7267/api/orders/all')
            .subscribe({
              next: orders => {
                const updatedOrder = orders.find(o => o.orderId === order.orderId);
                if (updatedOrder) {
                  order.status = updatedOrder.status;
                  order.paymentStatus = updatedOrder.paymentStatus;
                  order.totalAmount = updatedOrder.totalAmount;
                  order.orderDate = updatedOrder.orderDate;
                }
                this.snack.open(`Order ${order.orderId} status updated to ${newStatus}`, 'Close', { duration: 3000 });
                this.cd.detectChanges();
              },
              error: () => {
                order.status = previousStatus;
                this.snack.open(`Failed to refresh order after update`, 'Close', { duration: 3000 });
              }
            });
        },
        error: err => {
          // rollback visually on failure
          this.http.get<OrderListDto[]>('https://localhost:7267/api/orders/all')
            .subscribe({
              next: orders => {
                const actualOrder = orders.find(o => o.orderId === order.orderId);
                if (actualOrder) {
                  order.status = actualOrder.status;
                  order.paymentStatus = actualOrder.paymentStatus;
                  order.totalAmount = actualOrder.totalAmount;
                  order.orderDate = actualOrder.orderDate;
                } else order.status = previousStatus;
                this.snack.open(`Failed to update status: ${err.error?.message || err.message}`, 'Close', { duration: 4000 });
                this.cd.detectChanges();
              },
              error: () => {
                order.status = previousStatus;
              }
            });
        }
      });
  }
}
