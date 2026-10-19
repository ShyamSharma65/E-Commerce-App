import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { finalize } from 'rxjs';
import {
  CreateProductRequest,
  Cutomerservice,
  OrderStatus,
  OrderSummary,
  Product,
  ProductCategory
} from '../customer/service/cutomerservice';

type AdminSection = 'catalog' | 'orders';

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, RouterLink],
  templateUrl: './admin-dashboard.html',
  styleUrl: './admin-dashboard.css'
})
export class AdminDashboard implements OnInit {
  readonly statuses: OrderStatus[] = ['Pending', 'Processing', 'Shipped', 'Delivered', 'Canceled'];
  activeSection: AdminSection = 'catalog';
  products: Product[] = [];
  categories: ProductCategory[] = [];
  orders: OrderSummary[] = [];
  selectedStatuses: Record<number, OrderStatus> = {};
  isLoadingProducts = false;
  isLoadingOrders = false;
  isSavingProduct = false;
  savingOrderId: number | null = null;
  successMessage = '';
  errorMessage = '';

  productForm = new FormGroup({
    name: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.minLength(3), Validators.maxLength(100)] }),
    description: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.minLength(10)] }),
    price: new FormControl<number | null>(null, [Validators.required, Validators.min(0.01), Validators.max(10000)]),
    stockQuantity: new FormControl<number | null>(0, [Validators.required, Validators.min(0), Validators.max(1000)]),
    imageUrl: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.maxLength(2048)] }),
    discountPercentage: new FormControl<number | null>(0, [Validators.required, Validators.min(0), Validators.max(100)]),
    isAvailable: new FormControl(true, { nonNullable: true }),
    categoryId: new FormControl<number | null>(null, [Validators.required, Validators.min(1)])
  });

  constructor(
    private customerService: Cutomerservice,
    private router: Router,
    private changeDetector: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadProducts();
    this.loadCategories();
    this.loadOrders();
  }

  showSection(section: AdminSection): void {
    this.activeSection = section;
    this.successMessage = '';
    this.errorMessage = '';
  }

  createProduct(): void {
    if (this.productForm.invalid) {
      this.productForm.markAllAsTouched();
      return;
    }

    const values = this.productForm.getRawValue();
    const request: CreateProductRequest = {
      name: values.name.trim(),
      description: values.description.trim(),
      price: values.price ?? 0,
      stockQuantity: values.stockQuantity ?? 0,
      imageUrl: values.imageUrl.trim(),
      discountPercentage: values.discountPercentage ?? 0,
      isAvailable: values.isAvailable,
      categoryId: values.categoryId ?? 0
    };

    this.isSavingProduct = true;
    this.clearMessages();
    this.customerService.createProduct(request).subscribe({
      next: (product) => {
        this.products = [product, ...this.products];
        this.successMessage = `${product.name} was added to the catalog.`;
        this.productForm.reset({
          name: '',
          description: '',
          price: null,
          stockQuantity: 0,
          imageUrl: '',
          discountPercentage: 0,
          isAvailable: true,
          categoryId: null
        });
        this.isSavingProduct = false;
      },
      error: (error: unknown) => {
        this.errorMessage = this.getErrorMessage(error, 'Product could not be saved.');
        this.isSavingProduct = false;
      }
    });
  }

  setOrderStatus(orderId: number, status: OrderStatus): void {
    this.selectedStatuses[orderId] = status;
  }

  saveOrderStatus(order: OrderSummary): void {
    const status = this.selectedStatuses[order.id];
    if (!status || status === order.status) {
      return;
    }

    this.savingOrderId = order.id;
    this.clearMessages();
    this.customerService.updateOrderStatus(order.id, status).pipe(
      finalize(() => {
        this.savingOrderId = null;
        this.changeDetector.markForCheck();
      })
    ).subscribe({
      next: (updatedOrder) => {
        this.orders = this.orders.map((current) => current.id === updatedOrder.id ? updatedOrder : current);
        this.selectedStatuses[updatedOrder.id] = status;
        this.successMessage = `Order ${updatedOrder.orderNumber} is now ${updatedOrder.status.toLowerCase()}.`;
        this.changeDetector.markForCheck();
      },
      error: (error: unknown) => {
        this.errorMessage = this.getErrorMessage(error, 'Order status could not be updated.');
        this.changeDetector.markForCheck();
      }
    });
  }

  logout(): void {
    this.customerService.logout();
    this.router.navigate(['/login']);
  }

  private loadProducts(): void {
    this.isLoadingProducts = true;
    this.customerService.getProducts().subscribe({
      next: (products) => {
        this.products = products;
        this.isLoadingProducts = false;
      },
      error: () => {
        this.errorMessage = 'Catalog could not be loaded.';
        this.isLoadingProducts = false;
      }
    });
  }

  private loadCategories(): void {
    this.customerService.getProductCategories().subscribe({
      next: (categories) => this.categories = categories,
      error: () => this.errorMessage = 'Product categories could not be loaded.'
    });
  }

  private loadOrders(): void {
    this.isLoadingOrders = true;
    this.customerService.getAllOrders().subscribe({
      next: (orders) => {
        this.orders = orders;
        this.selectedStatuses = Object.fromEntries(orders.map((order) => [order.id, order.status as OrderStatus]));
        this.isLoadingOrders = false;
      },
      error: () => {
        this.errorMessage = 'Orders could not be loaded.';
        this.isLoadingOrders = false;
      }
    });
  }

  private clearMessages(): void {
    this.successMessage = '';
    this.errorMessage = '';
  }

  private getErrorMessage(error: unknown, fallback: string): string {
    const response = error as { error?: { message?: string; title?: string; errors?: Record<string, string[]> } };
    const firstValidationError = response?.error?.errors
      ? Object.values(response.error.errors).flat()[0]
      : undefined;

    return response?.error?.message ?? firstValidationError ?? response?.error?.title ?? fallback;
  }
}