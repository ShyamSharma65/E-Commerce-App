import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { RegisterData } from '../Models/registerCustomer';

export interface Product {
  id: number;
  name: string;
  description: string;
  price: number;
  stockQuantity: number;
  imageUrl: string;
  categoryName: string;
  categoryId: number;
  discountPercentage: number;
  isAvailable: boolean;
}

export interface ProductCategory {
  id: number;
  name: string;
}

export interface CreateProductRequest {
  name: string;
  description: string;
  price: number;
  stockQuantity: number;
  imageUrl: string;
  discountPercentage: number;
  isAvailable: boolean;
  categoryId: number;
}

export interface CartItem {
  productId: number;
  productName: string;
  unitPrice: number;
  quantity: number;
  totalPrice: number;
}

export interface CartSummary {
  cartId: number;
  items: CartItem[];
  totalAmount: number;
  totalItems: number;
}

export interface OrderSummary {
  id: number;
  orderNumber: string;
  totalAmount: number;
  status: string;
  createdAt: string;
}

export type OrderStatus = 'Pending' | 'Processing' | 'Shipped' | 'Delivered' | 'Canceled';

@Injectable({
  providedIn: 'root',
})
export class Cutomerservice {
  private readonly apiUrl = 'http://localhost:5000/api';

  constructor(private http: HttpClient) {}

  private getToken(): string | null {
    if (typeof sessionStorage === 'undefined') {
      return null;
    }

    return sessionStorage.getItem('ecommerce_token');
  }

  isAuthenticated(): boolean {
    return !!this.getToken();
  }

  isAdmin(): boolean {
    const token = this.getToken();
    if (!token) {
      return false;
    }

    try {
      const payloadSegment = token.split('.')[1];
      if (!payloadSegment) {
        return false;
      }

      const base64Payload = payloadSegment.replace(/-/g, '+').replace(/_/g, '/');
      const payload = JSON.parse(atob(base64Payload)) as Record<string, unknown>;
      const role = payload['role']
        ?? payload['roles']
        ?? payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];

      return Array.isArray(role) ? role.includes('Admin') : role === 'Admin';
    } catch {
      return false;
    }
  }

  storeToken(token: string): void {
    sessionStorage.setItem('ecommerce_token', token);
  }

  logout(): void {
    sessionStorage.removeItem('ecommerce_token');
  }

  private buildAuthHeaders(): HttpHeaders {
    const token = this.getToken();
    return token ? new HttpHeaders({ Authorization: `Bearer ${token}` }) : new HttpHeaders();
  }

  registerCustomer(data: RegisterData): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/auth/register`, {
      firstName: data.firstName,
      middleName: data.middleName,
      lastName: data.lastName,
      email: data.email,
      phone: data.phone,
      dateOfBirth: data.dob,
      password: data.password
    });
  }

  registerAdmin(data: RegisterData): Observable<{ id: number; email: string; fullName: string; role: string; message: string }> {
    return this.http.post<{ id: number; email: string; fullName: string; role: string; message: string }>(
      `${this.apiUrl}/auth/register-admin`,
      {
        firstName: data.firstName,
        middleName: data.middleName,
        lastName: data.lastName,
        email: data.email,
        phone: data.phone,
        dateOfBirth: data.dob,
        password: data.password
      },
      { headers: this.buildAuthHeaders() }
    );
  }

  login(data: { email: string; password: string }): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/auth/login`, data);
  }

  getProducts(): Observable<Product[]> {
    return this.http.get<Product[]>(`${this.apiUrl}/product`);
  }

  getProductCategories(): Observable<ProductCategory[]> {
    return this.http.get<ProductCategory[]>(`${this.apiUrl}/category`);
  }

  createProduct(request: CreateProductRequest): Observable<Product> {
    return this.http.post<Product>(`${this.apiUrl}/product`, request, { headers: this.buildAuthHeaders() });
  }

  getCart(): Observable<CartSummary> {
    return this.http.get<CartSummary>(`${this.apiUrl}/cart`, { headers: this.buildAuthHeaders() });
  }

  addToCart(productId: number, quantity = 1): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/cart/items`, { productId, quantity }, { headers: this.buildAuthHeaders() });
  }

  removeFromCart(productId: number): Observable<any> {
    return this.http.delete<any>(`${this.apiUrl}/cart/items/${productId}`, { headers: this.buildAuthHeaders() });
  }

  placeOrder(request: any): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/order`, request, { headers: this.buildAuthHeaders() });
  }

  getOrders(): Observable<OrderSummary[]> {
    return this.http.get<OrderSummary[]>(`${this.apiUrl}/order/my-orders`, { headers: this.buildAuthHeaders() });
  }

  getAllOrders(): Observable<OrderSummary[]> {
    return this.http.get<OrderSummary[]>(`${this.apiUrl}/order/all`, { headers: this.buildAuthHeaders() });
  }

  updateOrderStatus(orderId: number, status: OrderStatus): Observable<OrderSummary> {
    return this.http.patch<OrderSummary>(`${this.apiUrl}/order/${orderId}/status`, { status }, { headers: this.buildAuthHeaders() });
  }
}
