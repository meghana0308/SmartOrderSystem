import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { InventoryProduct } from '../../models/inventory-product';
import { AuthService } from './auth-service'; // a service storing JWT

@Injectable({ providedIn: 'root' })
export class InventoryService {
  constructor(private http: HttpClient, private authService: AuthService) {}

  private getHeaders() {
    const token = this.authService.getToken(); // your JWT
    return { headers: new HttpHeaders({ Authorization: `Bearer ${token}` }) };
  }

  getAllProducts() {
    return this.http.get<InventoryProduct[]>('https://localhost:7267/api/inventory/products', this.getHeaders());
  }

  getLowStock() {
    return this.http.get<InventoryProduct[]>('https://localhost:7267/api/inventory/lowstock', this.getHeaders());
  }

  updateStock(productId:number, stockQuantity:number) {
    return this.http.put(`https://localhost:7267/api/inventory/${productId}/stock`, { stockQuantity }, this.getHeaders());
  }

  updateReorderLevel(productId:number, reorderLevel:number) {
    return this.http.put(`https://localhost:7267/api/inventory/${productId}/reorder-level`, { reorderLevel }, this.getHeaders());
  }
}
