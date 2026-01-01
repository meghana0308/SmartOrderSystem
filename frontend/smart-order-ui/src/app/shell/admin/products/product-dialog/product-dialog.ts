import { Component, Inject, OnInit } from '@angular/core'
import { CommonModule } from '@angular/common'
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog'
import { MatFormFieldModule } from '@angular/material/form-field'
import { MatInputModule } from '@angular/material/input'
import { MatButtonModule } from '@angular/material/button'
import { MatSelectModule } from '@angular/material/select'
import { FormsModule } from '@angular/forms'
import { ProductService } from '../../../../core/services/product-service'
import { CategoryService, Category } from '../../../../core/services/category-service'

@Component({
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatSelectModule,
    MatDialogModule   
  ],
  templateUrl: './product-dialog.html'
})
export class ProductDialogComponent implements OnInit {

  name = ''
  unitPrice = 0
  categoryId!: number

  categories: Category[] = []

  constructor(
    private dialogRef: MatDialogRef<ProductDialogComponent>,
    private productService: ProductService,
    private categoryService: CategoryService,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {}

  ngOnInit() {
    this.loadCategories()

    if (this.data?.product) {
      this.name = this.data.product.name
      this.unitPrice = this.data.product.unitPrice
      this.categoryId = this.data.product.categoryId
    }
  }

  loadCategories() {
    this.categoryService.getAll().subscribe(res => {
      this.categories = res
    })
  }

  save() {
    const payload = {
      name: this.name,
      unitPrice: this.unitPrice,
      categoryId: this.categoryId
    }

    if (this.data?.product) {
      this.productService
        .update(this.data.product.id, payload)
        .subscribe(() => this.dialogRef.close(true))
    } else {
      this.productService
        .create(payload)
        .subscribe(() => this.dialogRef.close(true))
    }
  }

  cancel() {
    this.dialogRef.close()
  }
}
