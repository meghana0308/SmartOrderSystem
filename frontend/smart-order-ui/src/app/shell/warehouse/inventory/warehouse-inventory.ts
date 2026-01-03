import { AfterViewInit, Component, ViewChild } from '@angular/core'
import { CommonModule } from '@angular/common'
import { FormsModule } from '@angular/forms'
import { MatTableDataSource, MatTableModule } from '@angular/material/table'
import { MatSort, MatSortModule } from '@angular/material/sort'
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator'
import { MatFormFieldModule } from '@angular/material/form-field'
import { MatInputModule } from '@angular/material/input'
import { MatButtonModule } from '@angular/material/button'
import { InventoryService } from '../../../core/services/inventory-service'
import { InventoryProduct } from '../../../models/inventory-product'

@Component({
  standalone: true,
  selector: 'app-warehouse-inventory',
  imports: [
    CommonModule,
    FormsModule,
    MatTableModule,
    MatSortModule,
    MatPaginatorModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule
  ],
  templateUrl: './warehouse-inventory.html',
  styleUrls: ['./warehouse-inventory.css']
})
export class WarehouseInventoryComponent implements AfterViewInit {

  displayedColumns = ['name', 'categoryName', 'stockQuantity', 'reorderLevel', 'actions']
  dataSource = new MatTableDataSource<InventoryProduct>([])

  selectedProduct: InventoryProduct | null = null
  newStock = 0
  newReorderLevel = 0

  @ViewChild(MatSort) sort!: MatSort
  @ViewChild(MatPaginator) paginator!: MatPaginator

  constructor(private inventoryService: InventoryService) {
    this.loadProducts()
  }

  ngAfterViewInit() {
    this.dataSource.sort = this.sort
    this.dataSource.paginator = this.paginator
  }

  loadProducts() {
    this.inventoryService.getAllProducts().subscribe(r => {
      this.dataSource.data = r
    })
  }

  applyFilter(event: any) {
    const value = event.target.value?.trim().toLowerCase()
    this.dataSource.filter = value
  }

  openPopup(product: InventoryProduct) {
    this.selectedProduct = { ...product }
    this.newStock = product.stockQuantity
    this.newReorderLevel = product.reorderLevel
    ;(document.getElementById('stockDialog') as any).showModal()
  }

  closePopup() {
    ;(document.getElementById('stockDialog') as any).close()
    this.selectedProduct = null
  }

  saveChanges() {
    if (!this.selectedProduct) return

    this.inventoryService
      .updateStock(this.selectedProduct.productId, this.newStock)
      .subscribe(() => {
        this.inventoryService
          .updateReorderLevel(this.selectedProduct!.productId, this.newReorderLevel)
          .subscribe(() => {
            this.loadProducts()
            this.closePopup()
          })
      })
  }
}
