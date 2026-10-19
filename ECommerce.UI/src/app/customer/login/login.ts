import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { Cutomerservice } from '../service/cutomerservice';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class Login {
  errorMessage = '';

  loginForm = new FormGroup({
    email: new FormControl('', [Validators.required, Validators.email]),
    password: new FormControl('', [Validators.required])
  });

  constructor(private router: Router, private customerService: Cutomerservice) {}

  submitLogin(): void {
    if (this.loginForm.invalid) {
      this.errorMessage = 'Please enter a valid email and password.';
      return;
    }

    const value = this.loginForm.value;
    this.customerService.login({
      email: value.email ?? '',
      password: value.password ?? ''
    }).subscribe({
      next: (response) => {
        const token = response?.token ?? response?.Token ?? response?.data?.token ?? response?.data?.Token;

        if (token) {
          this.customerService.storeToken(token);
          this.router.navigate([this.customerService.isAdmin() ? '/admin' : '/']);
          return;
        }

        this.errorMessage = 'Login response did not contain a token.';
      },
      error: (err) => {
        this.errorMessage = err?.error?.message || 'Login failed. Please try again.';
      }
    });
  }

  registerAccount(): void {
    this.router.navigate(['register']);
  }
}
