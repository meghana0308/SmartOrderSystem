import { Component, OnInit } from '@angular/core'
import { CommonModule, DatePipe, CurrencyPipe } from '@angular/common'
import { MatTableModule } from '@angular/material/table'
import { SalesReportsService } from '../../../core/services/sales-reports-service'
import { ProductSalesReportDto } from '../../../models/product-sales-report'
import { FormsModule } from '@angular/forms'

@Component({
  standalone:true,
  selector:'app-sales-reports',
  imports:[CommonModule,MatTableModule,DatePipe,CurrencyPipe, FormsModule],
  templateUrl:'./sales-reports.html'
})
export class SalesReportsComponent implements OnInit{

  fromDate=''
  toDate=''

  salesByDate:any[]=[]
  salesByProduct:ProductSalesReportDto[]=[]
  topProducts:ProductSalesReportDto[]=[]
  lowProducts:ProductSalesReportDto[]=[]

  totalOrders=0
  totalRevenue=0

  constructor(private service:SalesReportsService){}

  ngOnInit(){
    const today=new Date().toISOString().split('T')[0]
    this.fromDate=today
    this.toDate=today
    this.loadAll()
  }

  loadAll(){
    this.loadSalesByDate()
    this.loadSalesByProduct()
    this.loadTopProducts()
    this.loadLowProducts()
  }

  loadSalesByDate(){
    this.service.getSalesByDate(this.fromDate,this.toDate)
      .subscribe(res=>{
        this.salesByDate=res
        this.totalOrders=res.reduce((s,x)=>s+x.ordersCount,0)
        this.totalRevenue=res.reduce((s,x)=>s+x.totalSales,0)
      })
  }

  loadSalesByProduct(){
    this.service.getSalesByProduct()
      .subscribe(res=>this.salesByProduct=res)
  }

  loadTopProducts(){
    this.service.getTopSellingProducts(5)
      .subscribe(res=>this.topProducts=res)
  }

  loadLowProducts(){
    this.service.getLowSellingProducts(5)
      .subscribe(res=>this.lowProducts=res)
  }
}
