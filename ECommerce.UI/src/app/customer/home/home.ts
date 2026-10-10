import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { CartSummary, Cutomerservice, Product } from '../service/cutomerservice';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './home.html',
  styleUrl: './home.css',
})
export class Home implements OnInit {
  products: Product[] = [];
  filteredProducts: Product[] = [];
  cart: CartSummary = { cartId: 0, items: [], totalAmount: 0, totalItems: 0 };
  orders: any[] = [];
  isLoggedIn = false;
  statusMessage = '';
  selectedCategory = 'All';
  categories = ['All', 'Electronics', 'Books', 'Best Sellers', 'New Arrivals'];

  constructor(private router: Router, private customerService: Cutomerservice) {}

  ngOnInit(): void {
    this.isLoggedIn = this.customerService.isAuthenticated();
    if (!this.isLoggedIn) {
      return;
    }

    this.loadPageData();
  }

  applyCategoryFilter(): void {
    if (!this.products.length) {
      this.filteredProducts = [];
      return;
    }

    if (this.selectedCategory === 'All') {
      this.filteredProducts = this.products;
      return;
    }

    this.filteredProducts = this.products.filter((product) => {
      if (this.selectedCategory === 'Best Sellers') {
        return product.stockQuantity >= 30;
      }

      if (this.selectedCategory === 'New Arrivals') {
        return product.stockQuantity > 0;
      }

      return product.categoryName === this.selectedCategory;
    });
  }

  loadPageData(): void {
    this.customerService.getProducts().subscribe((products) => {
      this.products = products;
      this.applyCategoryFilter();
    });

    this.customerService.getCart().subscribe({
      next: (cart) => {
        this.cart = cart;
      },
      error: () => {
        this.customerService.logout();
        this.router.navigate(['login']);
      }
    });

    this.customerService.getOrders().subscribe({
      next: (orders) => {
        this.orders = orders;
      },
      error: () => {
        this.orders = [];
      }
    });
  }

  gotoLogin() {
    this.router.navigate(['login']);
  }

  gotoRegister() {
    this.router.navigate(['register']);
  }

  chooseCategory(category: string): void {
    this.selectedCategory = category;
    this.applyCategoryFilter();
  }

  addToCart(product: Product): void {
    if (!this.customerService.isAuthenticated()) {
      this.router.navigate(['login']);
      return;
    }

    this.customerService.addToCart(product.id, 1).subscribe({
      next: () => {
        this.statusMessage = `${product.name} added to cart.`;
        this.loadPageData();
      },
      error: () => {
        this.statusMessage = 'Unable to add item to cart.';
      }
    });
  }

  placeOrder(): void {
    if (!this.cart.items.length) {
      this.statusMessage = 'Your cart is empty.';
      return;
    }

    const payload = {
      shippingAddress: 'Main Street',
      billingAddress: 'Main Street',
      city: 'Default City',
      state: 'Default',
      postalCode: '00000',
      country: 'India'
    };

    this.customerService.placeOrder(payload).subscribe({
      next: (response) => {
        this.statusMessage = `Order placed successfully! Order #${response.orderNumber}`;
        this.loadPageData();
      },
      error: () => {
        this.statusMessage = 'Unable to place your order right now.';
      }
    });
  }

  logout(): void {
    this.customerService.logout();
    this.isLoggedIn = false;
    this.router.navigate(['login']);
  }
}
