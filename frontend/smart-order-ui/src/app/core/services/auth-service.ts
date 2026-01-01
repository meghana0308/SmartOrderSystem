import { Injectable, inject } from '@angular/core'
import { HttpClient } from '@angular/common/http'
import { Observable } from 'rxjs'
import { PLATFORM_ID } from '@angular/core'
import { isPlatformBrowser } from '@angular/common'

@Injectable({ providedIn: 'root' })
export class AuthService {

  private baseUrl = 'https://localhost:7267/api/auth'
  private platformId = inject(PLATFORM_ID)

  constructor(private http: HttpClient) {}

  private isBrowser() {
    return isPlatformBrowser(this.platformId)
  }

  login(data: { email: string; password: string }) {
    return this.http.post<{ token: string }>(`${this.baseUrl}/login`, data)
  }

  register(data: { fullName: string; email: string; password: string }) {
  // Map to PascalCase for backend
  const payload = {
    FullName: data.fullName,
    Email: data.email,
    Password: data.password
  };
  return this.http.post(`${this.baseUrl}/register`, payload, { responseType: 'text' });
}


  saveToken(token: string) {
    if (this.isBrowser()) {
      localStorage.setItem('auth_token', token)
    }
  }

  getToken(): string | null {
    return this.isBrowser() ? localStorage.getItem('auth_token') : null
  }

  isLoggedIn(): boolean {
    return !!this.getToken()
  }

  getRole(): string {
    const token = this.getToken()
    if (!token) return ''

    const payload = JSON.parse(atob(token.split('.')[1]))

    return (
      payload.role ||
      payload.roles?.[0] ||
      payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] ||
      ''
    )
  }

  logout() {
    if (this.isBrowser()) {
      localStorage.removeItem('auth_token')
    }
  }
}
