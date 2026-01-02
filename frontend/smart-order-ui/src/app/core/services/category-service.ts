import { Injectable } from '@angular/core'
import { HttpClient } from '@angular/common/http'
import { Observable, throwError } from 'rxjs'
import { catchError } from 'rxjs/operators'

export interface Category {
  id: number
  name: string
}

@Injectable({ providedIn: 'root' })
export class CategoryService {

  private apiUrl = 'https://localhost:7267/api/admin/categories'

  constructor(private http: HttpClient) {}

  getAll(): Observable<Category[]> {
    return this.http.get<Category[]>(this.apiUrl)
      .pipe(catchError(err => throwError(() => err.error || err)))
  }
  create(payload: { name: string }) {
  return this.http.post(this.apiUrl, payload).pipe(catchError(err => throwError(() => err.error || err)))
}

update(id: number, payload: { name: string }) {
  return this.http.put(`${this.apiUrl}/${id}`, payload)
      .pipe(catchError(err => throwError(() => err.error || err)))
}

delete(id: number) {
  return this.http.delete(`${this.apiUrl}/${id}`)
      .pipe(catchError(err => throwError(() => err.error || err)))
}

}
