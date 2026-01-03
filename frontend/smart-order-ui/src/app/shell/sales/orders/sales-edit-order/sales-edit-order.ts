import { Component, inject } from '@angular/core';
import { CommonModule, CurrencyPipe } from '@angular/common';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormsModule } from '@angular/forms';
import { SalesOrderService } from '../../../../core/services/sales-order-service';
import { ProductService, Product } from '../../../../core/services/product-service';
import { OrderListDto } from '../../../../models/sales-order.model';

type PaymentMode = 'PayLater' | 'PayNow';

@Component({
  standalone: true,
  selector: 'app-edit-order-dialog',
  imports: [CommonModule, FormsModule, MatDialogModule, CurrencyPipe],
  template: `
<h2>Edit Order #{{data.order.orderId}}</h2>

<div *ngFor="let item of orderItems; let i = index"
     style="margin-bottom:8px; display:flex; align-items:center;">

  <select [(ngModel)]="item.productId" (ngModelChange)="onProductChange(i)">
    <option *ngFor="let p of products" [ngValue]="p.id">
      {{p.name}}
    </option>
  </select>

  <input type="number"
         [(ngModel)]="item.quantity"
         (ngModelChange)="updateTotal()"
         min="1"
         style="width:60px; margin-left:8px"/>

  <span style="margin-left:8px;">
    Unit: {{item.unitPrice | currency:'INR'}}
  </span>

  <span style="margin-left:8px;">
    Total: {{item.unitPrice * item.quantity | currency:'INR'}}
  </span>

  <button (click)="removeItem(i)" style="margin-left:8px;">Remove</button>
</div>

<button (click)="addItem()" style="margin-top:8px;">Add Product</button>

<div style="margin-top:16px;">
  <label>Payment Mode:</label>
  <select [(ngModel)]="paymentMode">
    <option value="PayNow">Pay Now</option>
    <option value="PayLater">Pay Later</option>
  </select>
</div>

<div style="margin-top:16px;">
  <strong>Total Amount: {{totalAmount | currency:'INR'}}</strong>
</div>

<div style="margin-top:16px;">
  <button (click)="save()">Save</button>
  <button (click)="close()">Cancel</button>
</div>
  `
})
export class EditOrderDialogComponent {

  data = inject(MAT_DIALOG_DATA) as { order: OrderListDto };
  dialogRef = inject(MatDialogRef<EditOrderDialogComponent>);
  orderService = inject(SalesOrderService);
  productService = inject(ProductService);

  products: Product[] = [];
  orderItems: { productId: number; quantity: number; unitPrice: number }[] = [];
  totalAmount = 0;
  paymentMode: PaymentMode = 'PayLater';

  constructor() {
    this.productService.getAll().subscribe(products => {
      this.products = products;

      this.orderItems = this.data.order.items.map(i => {
        const prod = this.products.find(p => p.id === i.productId);
        return {
          productId: i.productId,
          quantity: i.quantity,
          unitPrice: prod?.unitPrice ?? 0
        };
      });

      this.paymentMode = (this.data.order as any).paymentMode ?? 'PayLater';
      this.updateTotal();
    });
  }

  onProductChange(index: number) {
    const productId = this.orderItems[index].productId;
    const prod = this.products.find(p => p.id === productId);

    if (prod) {
      this.orderItems[index].unitPrice = prod.unitPrice;
      this.updateTotal();
    }
  }

  updateTotal() {
    this.totalAmount = this.orderItems
      .reduce((sum, i) => sum + i.unitPrice * i.quantity, 0);
  }

  addItem() {
    if (!this.products.length) return;

    const p = this.products[0];
    this.orderItems.push({
      productId: p.id,
      quantity: 1,
      unitPrice: p.unitPrice
    });
    this.updateTotal();
  }

  removeItem(index: number) {
    this.orderItems.splice(index, 1);
    this.updateTotal();
  }

  save() {
    const payload = {
      items: this.orderItems.map(i => ({
        productId: i.productId,
        quantity: i.quantity
      })),
      paymentMode: this.paymentMode
    };

    this.orderService.updateOrder(this.data.order.orderId, payload).subscribe({
      next: () => this.dialogRef.close(true),
      error: err => alert('Update failed')
    });
  }

  close() {
    this.dialogRef.close(false);
  }
}
