import { Injectable } from '@angular/core'
import { HttpClient, HttpParams } from '@angular/common/http'
import { Observable } from 'rxjs'
import { ProductSalesReportDto } from '../../models/product-sales-report'

@Injectable({ providedIn:'root' })
export class SalesReportsService{

  private baseUrl='https://localhost:7267/api/reports'

  constructor(private http:HttpClient){}

  getSalesByDate(fromDate:string,toDate:string):Observable<any[]>{
    const params=new HttpParams()
      .set('fromDate',fromDate)
      .set('toDate',toDate)

    return this.http.get<any[]>(`${this.baseUrl}/sales/by-date`,{params})
  }

  getSalesByProduct():Observable<ProductSalesReportDto[]>{
    return this.http.get<ProductSalesReportDto[]>(`${this.baseUrl}/sales/by-product`)
  }

  getTopSellingProducts(top:number=10,fromDate?:string,toDate?:string):Observable<ProductSalesReportDto[]>{
    let params=new HttpParams().set('top',top)

    if(fromDate) params=params.set('fromDate',fromDate)
    if(toDate) params=params.set('toDate',toDate)

    return this.http.get<ProductSalesReportDto[]>(`${this.baseUrl}/sales/top-products`,{params})
  }

  getLowSellingProducts(top:number=10,fromDate?:string,toDate?:string):Observable<ProductSalesReportDto[]>{
    let params=new HttpParams().set('top',top)

    if(fromDate) params=params.set('fromDate',fromDate)
    if(toDate) params=params.set('toDate',toDate)

    return this.http.get<ProductSalesReportDto[]>(`${this.baseUrl}/sales/low-selling`,{params})
  }
}
