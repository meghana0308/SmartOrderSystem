import { Component, OnInit, inject } from '@angular/core'
import { FormBuilder, FormArray, ReactiveFormsModule, Validators } from '@angular/forms'
import { CommonModule, CurrencyPipe } from '@angular/common'
import { ProductService } from '../../../core/services/product-service'
import { OrderService } from '../../../core/services/order-service'
import { CreateOrderDto } from '../../../models/order.model'

@Component({
  standalone: true,
  selector: 'app-create-order',
  templateUrl: './create-order.html',
  imports: [CommonModule, ReactiveFormsModule, CurrencyPipe]
})
export class CreateOrderComponent implements OnInit {
  products: any[] = []
  loadingProducts = false
  submitting = false

  private readonly fb = inject(FormBuilder)
  private readonly productService = inject(ProductService)
  private readonly orderService = inject(OrderService)

  readonly form = this.fb.group({
    items: this.fb.array([]),
    paymentMode: ['PayLater', Validators.required], // 'PayNow' | 'PayLater'
    customerId: [null] // optional
  })

  ngOnInit() {
    this.loadProducts()
    this.addItem()
  }

  get items() { return this.form.get('items') as FormArray }

  newItem() {
    return this.fb.group({
      productId: [null, Validators.required],
      quantity: [1, [Validators.required, Validators.min(1)]]
    })
  }

  addItem() { this.items.push(this.newItem()) }
  removeItem(index: number) { this.items.removeAt(index) }

  loadProducts() {
    this.loadingProducts = true
    this.productService.getAll().subscribe({
      next: (res) => { this.products = res; this.loadingProducts = false },
      error: () => { this.loadingProducts = false }
    })
  }

  getItemTotal(item: any) {
    const product = this.products.find(p => p.id === item.get('productId')?.value)
    return (product?.unitPrice ?? 0) * item.get('quantity')?.value
  }

  get totalAmount() {
    return this.items.controls.reduce((sum, item) => sum + this.getItemTotal(item), 0)
  }

  submit() {
    if (this.form.invalid) return
    this.submitting = true

    const formValue = this.form.value
    const mode: 'PayNow' | 'PayLater' = formValue.paymentMode === 'PayNow' ? 'PayNow' : 'PayLater'

    const payload: CreateOrderDto = {
      items: (formValue.items ?? []).map((i: any) => ({
  productId: Number(i.productId),
  quantity: Number(i.quantity)
})),

      paymentMode: mode
    }

    if (formValue.customerId) payload.customerId = formValue.customerId

    this.orderService.createOrder(payload).subscribe({
      next: (res) => {
        alert(res.message)
        this.form.reset({ paymentMode: 'PayLater', customerId: null })
        this.items.clear()
        this.addItem()
        this.submitting = false
      },
      error: (err) => {
        alert(err.error?.message || 'Error creating order')
        this.submitting = false
      }
    })
  }
}
