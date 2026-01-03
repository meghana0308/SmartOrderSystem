import { Injectable } from '@angular/core'
import { HttpClient } from '@angular/common/http'
import { Observable, throwError } from 'rxjs'
import { catchError } from 'rxjs/operators'
import { OrderListDto } from '../../models/sales-order.model'

@Injectable({ providedIn:'root' })
export class SalesOrderService {

  private apiUrl='https://localhost:7267/api/orders/created'

  constructor(private http:HttpClient){}

  getCreatedOrders():Observable<OrderListDto[]>{
    return this.http.get<OrderListDto[]>(this.apiUrl)
      .pipe(catchError(err=>throwError(()=>err.error||err)))
  }
    cancelOrder(orderId: number) {
  return this.http.put(`https://localhost:7267/api/orders/${orderId}/cancel`, {})
    .pipe(catchError(err => throwError(() => err.error || err)))
}

updateOrder(orderId: number, payload: any) {
  return this.http.put(`https://localhost:7267/api/orders/${orderId}`, payload)
    .pipe(catchError(err => throwError(() => err.error || err)))
}

}
