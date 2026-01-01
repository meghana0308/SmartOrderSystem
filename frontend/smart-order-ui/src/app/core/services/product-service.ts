import { Injectable } from '@angular/core'
import { HttpClient } from '@angular/common/http'
import { Observable } from 'rxjs'

export interface Product {
  id: number
  name: string
  unitPrice: number
  categoryName?: string
}


@Injectable({
  providedIn: 'root'
})
export class ProductService {
  private apiUrl = 'https://localhost:7267/api/products'

  constructor(private http: HttpClient) {}

  getAll(): Observable<Product[]> {
    return this.http.get<Product[]>(this.apiUrl)
  }
   create(payload: any) {
    return this.http.post(this.apiUrl, payload)
  }

  update(id: number, payload: any) {
    return this.http.put(`${this.apiUrl}/${id}`, payload)
  }

  delete(id: number) {
    return this.http.delete(`${this.apiUrl}/${id}`)
  }
}
