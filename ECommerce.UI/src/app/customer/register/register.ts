import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { Cutomerservice } from '../service/cutomerservice';
import { RegisterData } from '../Models/registerCustomer';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './register.html',
  styleUrl: './register.css',
})
export class Register {
  registerData: RegisterData | null = null;
  errorMessage = '';

  constructor(private router: Router, private customerService: Cutomerservice) {}

  registerForm = new FormGroup({
    email: new FormControl('', [Validators.required, Validators.email]),
    firstName: new FormControl('', Validators.required),
    middleName: new FormControl(''),
    lastName: new FormControl('', Validators.required),
    phone: new FormControl('', Validators.required),
    dob: new FormControl('', Validators.required),
    password: new FormControl('', [Validators.required, Validators.minLength(6)]),
    confirmPassword: new FormControl('', [Validators.required, Validators.minLength(6)])
  });

  gotoLogin() {
    this.router.navigate(['login']);
  }

  register() {
    if (this.registerForm.invalid) {
      this.errorMessage = 'Please fill in all required fields.';
      return;
    }

    const formValue = this.registerForm.value;
    if (formValue.password !== formValue.confirmPassword) {
      this.errorMessage = 'Password and confirm password do not match.';
      return;
    }

    this.registerData = {
      firstName: formValue.firstName ?? '',
      middleName: formValue.middleName ?? '',
      lastName: formValue.lastName ?? '',
      email: formValue.email ?? '',
      phone: formValue.phone ?? '',
      dob: formValue.dob ?? '',
      password: formValue.password ?? ''
    };

    this.customerService.registerCustomer(this.registerData).subscribe({
      next: () => {
        this.router.navigate(['login']);
      },
      error: (err) => {
        this.errorMessage = err?.error?.message || 'Registration failed.';
      }
    });
  }
}

