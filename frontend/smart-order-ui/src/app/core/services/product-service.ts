import { Injectable } from '@angular/core'
import { HttpClient } from '@angular/common/http'
import { Observable, throwError } from 'rxjs'
import { catchError } from 'rxjs/operators'

export interface Product {
  id: number
  name: string
  unitPrice: number
  categoryName?: string
  categoryId?: number
}

@Injectable({ providedIn: 'root' })
export class ProductService {
  private apiUrl = 'https://localhost:7267/api/products'

  constructor(private http: HttpClient) {}

  getAll(): Observable<Product[]> {
    return this.http.get<Product[]>(this.apiUrl)
      .pipe(catchError(err => throwError(() => err.error || err)))
  }

  create(payload: any) {
    return this.http.post(this.apiUrl, payload)
      .pipe(catchError(err => throwError(() => err.error || err)))
  }

  update(id: number, payload: any) {
    return this.http.put(`${this.apiUrl}/${id}`, payload)
      .pipe(catchError(err => throwError(() => err.error || err)))
  }

  delete(id: number) {
    return this.http.delete(`${this.apiUrl}/${id}`)
      .pipe(catchError(err => throwError(() => err.error || err)))
  }
}
