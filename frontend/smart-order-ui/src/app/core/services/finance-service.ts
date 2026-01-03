import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { InvoiceListDto, InvoiceDetailDto, FinanceOrderDto } from '../../models/invoice.model';

@Injectable({ providedIn: 'root' })
export class FinanceService {
  private http = inject(HttpClient);
  private baseUrl = 'https://localhost:7267/api/invoices';

  getInvoices(paymentStatus?: string): Observable<InvoiceListDto[]> {
    let url = this.baseUrl;
    if (paymentStatus) url += `?paymentStatus=${paymentStatus}`;
    return this.http.get<InvoiceListDto[]>(url);
  }

  getInvoiceById(id: number): Observable<InvoiceDetailDto> {
    return this.http.get<InvoiceDetailDto>(`${this.baseUrl}/${id}`);
  }

  getInvoiceByOrder(orderId: number): Observable<InvoiceDetailDto> {
    return this.http.get<InvoiceDetailDto>(`${this.baseUrl}/order/${orderId}`);
  }

  getFinanceOrders(paymentStatus?: string): Observable<FinanceOrderDto[]> {
    let url = `${this.baseUrl}/finance/orders`;
    if (paymentStatus) url += `?paymentStatus=${paymentStatus}`;
    return this.http.get<FinanceOrderDto[]>(url);
  }
}
