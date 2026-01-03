import { Injectable, inject } from '@angular/core'
import { HttpClient, HttpParams } from '@angular/common/http'
import { Observable } from 'rxjs'
import {
  CreateOrderDto,
  OrderListDto,
  OrderQueryDto,
   UpdateOrderDto
} from '../../models/order.model'

@Injectable({ providedIn: 'root' })
export class OrderService {
  private readonly baseUrl = 'https://localhost:7267/api/orders'
  private readonly http = inject(HttpClient)

  getMyOrders(query?: OrderQueryDto): Observable<OrderListDto[]> {
    const q: OrderQueryDto = { page: 1, pageSize: 10, ...query }
    let params = new HttpParams()
      .set('page', q.page!.toString())
      .set('pageSize', q.pageSize!.toString())
    if (q.status) params = params.set('status', q.status)
    return this.http.get<OrderListDto[]>(`${this.baseUrl}/my`, { params })
  }

  getCreatedOrders(query?: OrderQueryDto): Observable<OrderListDto[]> {
    const q: OrderQueryDto = { page: 1, pageSize: 10, ...query }
    let params = new HttpParams()
      .set('page', q.page!.toString())
      .set('pageSize', q.pageSize!.toString())
    if (q.status) params = params.set('status', q.status)
    return this.http.get<OrderListDto[]>(`${this.baseUrl}/created`, { params })
  }

  createOrder(payload: CreateOrderDto) {
    return this.http.post<{ message: string; orderId: number }>(this.baseUrl, payload)
  }
   updateOrder(orderId: number, payload: UpdateOrderDto) {
    return this.http.put<{ message: string }>(`${this.baseUrl}/${orderId}`, payload)
  }


  cancelOrder(orderId: number) {
    return this.http.put<{ message: string }>(`${this.baseUrl}/${orderId}/cancel`, {})
  }
}
