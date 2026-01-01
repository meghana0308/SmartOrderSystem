import { AfterViewInit, Component, ViewChild } from '@angular/core'
import { CommonModule } from '@angular/common'
import { MatTableDataSource, MatTableModule } from '@angular/material/table'
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator'
import { MatSort, MatSortModule } from '@angular/material/sort'
import { MatFormFieldModule } from '@angular/material/form-field'
import { MatInputModule } from '@angular/material/input'
import { MatIconModule } from '@angular/material/icon'
import { MatButtonModule } from '@angular/material/button'
import { MatDialog, MatDialogModule } from '@angular/material/dialog'
import { ProductService, Product } from '../../../core/services/product-service'
import { ProductDialogComponent } from './product-dialog/product-dialog'

@Component({
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatFormFieldModule,
    MatInputModule,
    MatIconModule,
    MatButtonModule,
    MatDialogModule
  ],
  templateUrl: './products.html'
})
export class ProductsComponent implements AfterViewInit {

  displayedProductColumns = ['id', 'name', 'categoryName', 'unitPrice', 'actions']
  displayedCategoryColumns = ['categoryName', 'count']

  productsDS = new MatTableDataSource<Product>()
  categoriesDS = new MatTableDataSource<{ categoryName: string; count: number }>()

  @ViewChild(MatPaginator) paginator!: MatPaginator
  @ViewChild(MatSort) sort!: MatSort

  constructor(
    private productService: ProductService,
    private dialog: MatDialog
  ) {}

  ngAfterViewInit() {
    this.loadProducts()
  }

  loadProducts() {
    this.productService.getAll().subscribe(res => {
      this.productsDS.data = res
      this.productsDS.paginator = this.paginator
      this.productsDS.sort = this.sort
      this.buildCategories(res)
    })
  }

  buildCategories(products: Product[]) {
  const map = new Map<string, number>()

  products.forEach(p => {
    const key = p.categoryName ?? 'Uncategorized'
    map.set(key, (map.get(key) ?? 0) + 1)
  })

  this.categoriesDS.data = Array.from(map.entries()).map(
    ([categoryName, count]) => ({ categoryName, count })
  )
}


  applyFilter(e: any) {
    this.productsDS.filter = e.target.value.trim().toLowerCase()
  }

  openCreate() {
    this.dialog.open(ProductDialogComponent)
      .afterClosed()
      .subscribe(r => r && this.loadProducts())
  }

  openEdit(p: any) {
    this.dialog.open(ProductDialogComponent, { data: { product: p } })
      .afterClosed()
      .subscribe(r => r && this.loadProducts())
  }

  deleteProduct(id: number) {
    if (!confirm('Delete product?')) return
    this.productService.delete(id).subscribe(() => this.loadProducts())
  }
}
