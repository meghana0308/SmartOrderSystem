import { Component, OnInit, inject } from '@angular/core'
import { CommonModule, CurrencyPipe } from '@angular/common'
import { FormBuilder, FormArray, ReactiveFormsModule, Validators } from '@angular/forms'

import { ProductService } from '../../../../core/services/product-service'
import { OrderService } from '../../../../core/services/order-service'
import { CustomerLookupService } from '../../../../core/services/sales-only-service'
import { CreateOrderDto } from '../../../../models/order.model'
import { AdminUser } from '../../../../models/admin-user.model'
type PaymentMode = 'PayLater' | 'PayNow'

@Component({
  standalone: true,
  selector: 'app-sales-create-order',
  imports: [CommonModule, ReactiveFormsModule, CurrencyPipe],
  templateUrl: './sales-create-order.html'
})
export class SalesCreateOrderComponent implements OnInit {
    
  products: any[] = []
  customers: AdminUser[] = []

  loadingProducts = false
  loadingCustomers = false
  submitting = false

  private fb = inject(FormBuilder)
  private productService = inject(ProductService)
  private orderService = inject(OrderService)
  private customerService = inject(CustomerLookupService)


  readonly form = this.fb.group({
    customerId: [null, Validators.required],
    paymentMode: ['PayLater', Validators.required],
    items: this.fb.array([])
  })

  ngOnInit() {
    this.loadProducts()
    this.loadCustomers()
    this.addItem()
  }

  get items() {
    return this.form.get('items') as FormArray
  }

  newItem() {
    return this.fb.group({
      productId: [null, Validators.required],
      quantity: [1, [Validators.required, Validators.min(1)]]
    })
  }

  addItem() {
    this.items.push(this.newItem())
  }

  removeItem(index: number) {
    this.items.removeAt(index)
  }

  loadProducts() {
    this.loadingProducts = true
    this.productService.getAll().subscribe({
      next: res => {
        this.products = res
        this.loadingProducts = false
      },
      error: () => this.loadingProducts = false
    })
  }

loadCustomers() {
  this.loadingCustomers = true
  this.customerService.getCustomers().subscribe({
    next: res => {
      this.customers = res
      this.loadingCustomers = false
    },
    error: () => this.loadingCustomers = false
  })
}


  getItemTotal(item: any) {
    const product = this.products.find(p => p.id === item.get('productId')?.value)
    return (product?.unitPrice ?? 0) * item.get('quantity')?.value
  }

  get totalAmount() {
    return this.items.controls.reduce(
      (sum, item) => sum + this.getItemTotal(item),
      0
    )
  }

  submit() {
    if (this.form.invalid) return

    this.submitting = true
    const v = this.form.value

    const payload: CreateOrderDto = {
  customerId: v.customerId!,
  paymentMode: v.paymentMode as PaymentMode,
  items: (v.items ?? []).map((i: any) => ({
    productId: Number(i.productId),
    quantity: Number(i.quantity)
  }))
}


    this.orderService.createOrder(payload).subscribe({
      next: res => {
        alert(res.message)
        this.form.reset({ paymentMode: 'PayLater' })
        this.items.clear()
        this.addItem()
        this.submitting = false
      },
      error: err => {
        alert(err.error?.message || 'Failed to create order')
        this.submitting = false
      }
    })
  }
}
