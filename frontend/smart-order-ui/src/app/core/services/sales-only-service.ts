import { HttpClient } from "@angular/common/http"
import { Injectable } from "@angular/core"
import { catchError, throwError } from "rxjs";

@Injectable({ providedIn: 'root' })
export class CustomerLookupService {
  private apiUrl = 'https://localhost:7267/api/users/customers'

  constructor(private http: HttpClient) {}

  getCustomers() {
    return this.http.get<any[]>(this.apiUrl)
  }



}
