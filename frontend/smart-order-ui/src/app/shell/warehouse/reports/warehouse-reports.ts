import { Component } from '@angular/core'
import { CommonModule } from '@angular/common'
import { HttpClient } from '@angular/common/http'
import { ProductSalesReportDto } from '../../../models/product-sales-report'
import { InventoryProduct } from '../../../models/inventory-product'

@Component({
  standalone:true,
  imports:[CommonModule],
  templateUrl:'./warehouse-reports.html'
})
export class WarehouseReportsComponent {

  lowStock:InventoryProduct[]=[]
  topSelling:ProductSalesReportDto[]=[]
  lowSelling:ProductSalesReportDto[]=[]

  private api = 'https://localhost:7267/api'

  constructor(private http:HttpClient){
    this.loadAll()
  }

  loadAll(){
    this.http.get<any[]>(`${this.api}/inventory/lowstock`)
  .subscribe(r => {
    this.lowStock = r.map(x => ({
      productId: x.productId ?? x.ProductId,
      name: x.name ?? x.Name,
      categoryName: x.categoryName ?? x.CategoryName,
      stockQuantity: x.stockQuantity ?? x.StockQuantity,
      reorderLevel: x.reorderLevel ?? x.ReorderLevel
    }));
  });


    this.http.get<ProductSalesReportDto[]>(`${this.api}/reports/sales/top-products`)
      .subscribe(r=>this.topSelling=r)

    this.http.get<ProductSalesReportDto[]>(`${this.api}/reports/sales/low-selling`)
      .subscribe(r=>this.lowSelling=r)
  }
}
