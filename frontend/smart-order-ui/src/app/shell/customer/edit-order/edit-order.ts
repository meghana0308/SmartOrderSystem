import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core'
import { FormBuilder, FormArray, ReactiveFormsModule, Validators } from '@angular/forms'
import { CommonModule } from '@angular/common'
import { catchError, finalize, of } from 'rxjs'
import { OrderListDto, UpdateOrderDto } from '../../../models/order.model'
import { ProductService } from '../../../core/services/product-service'
import { OrderService } from '../../../core/services/order-service'

@Component({
  standalone: true,
  selector: 'app-edit-order',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './edit-order.html'
})
export class EditOrderComponent implements OnInit {
  @Input() order!: OrderListDto
  @Output() updated = new EventEmitter<void>()

  products: any[] = []
  errorMessage = ''
  submitting = false

  form: any

  constructor(
    private fb: FormBuilder,
    private productService: ProductService,
    private orderService: OrderService
  ) {
    this.form = this.fb.group({
      items: this.fb.array([]),
      paymentMode: ['PayLater']
    })
  }

  get items(): FormArray {
    return this.form.get('items') as FormArray
  }

  ngOnInit() {
    this.productService.getAll().subscribe(p => {
      this.products = p
      this.resetFormFromOrder()
    })
  }

  resetFormFromOrder() {
    this.items.clear()
    this.order.items.forEach(i => {
      this.items.push(this.fb.group({
        productId: [i.productId, Validators.required],
        quantity: [i.quantity, [Validators.required, Validators.min(1)]]
      }))
    })
  }

  addItem() {
    this.items.push(this.fb.group({
      productId: [null, Validators.required],
      quantity: [1, [Validators.required, Validators.min(1)]]
    }))
  }

  removeItem(index: number) {
    this.items.removeAt(index)
  }

  submit() {
    if (this.submitting) return

    this.errorMessage = ''

    if (this.form.invalid) {
      this.errorMessage = 'Fix form errors'
      return
    }

    const map = new Map<number, number>()
    for (const c of this.items.controls) {
      const pid = c.get('productId')!.value
      const qty = c.get('quantity')!.value
      if (!pid || qty <= 0) continue
      map.set(pid, (map.get(pid) || 0) + qty)
    }

    if (map.size === 0) {
      this.errorMessage = 'Order must contain at least one product'
      return
    }

    const payload: UpdateOrderDto = {
      items: Array.from(map.entries()).map(([productId, quantity]) => ({
        productId,
        quantity
      })),
      paymentMode: this.form.get('paymentMode')!.value as any
    }

    this.submitting = true

    this.orderService.updateOrder(this.order.orderId, payload)
      .pipe(
        catchError(err => {
          alert(err?.error?.message || 'Update failed')
          return of(null)
        }),
        finalize(() => {
          this.submitting = false
        })
      )
      .subscribe(res => {
        if (res) {
          alert('Order updated successfully')
          this.updated.emit()
        }
      })
  }
}
