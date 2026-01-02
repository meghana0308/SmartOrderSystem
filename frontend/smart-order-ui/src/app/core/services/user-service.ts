import { Injectable } from '@angular/core'
import { HttpClient } from '@angular/common/http'
import { Observable } from 'rxjs'
import { AdminUser } from '../../models/admin-user.model'

@Injectable({ providedIn: 'root' })
export class AdminUserService {
  private baseUrl = 'https://localhost:7267/api/admin'

  constructor(private http: HttpClient) {}

  getAllUsers(): Observable<AdminUser[]> {
    return this.http.get<AdminUser[]>(`${this.baseUrl}/users`)
  }

  changeUserRole(userId: string, role: string) {
  return this.http.put(
    `${this.baseUrl}/users/${userId}/role`,
    { role }
  )
}

}
