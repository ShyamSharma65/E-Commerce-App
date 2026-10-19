import { CommonModule } from '@angular/common';
import { afterNextRender, Component, signal } from '@angular/core';
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
  successMessage = '';
  readonly isAdmin = signal(false);

  constructor(private router: Router, private customerService: Cutomerservice) {
    afterNextRender(() => {
      this.isAdmin.set(this.customerService.isAdmin());
    });
  }

  registerForm = new FormGroup({
    email: new FormControl('', [Validators.required, Validators.email]),
    firstName: new FormControl('', Validators.required),
    middleName: new FormControl(''),
    lastName: new FormControl('', Validators.required),
    phone: new FormControl('', Validators.required),
    dob: new FormControl('', Validators.required),
    password: new FormControl('', [Validators.required, Validators.minLength(6)]),
    confirmPassword: new FormControl('', [Validators.required, Validators.minLength(6)]),
    accountRole: new FormControl<'Customer' | 'Admin'>('Customer', { nonNullable: true })
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

    this.errorMessage = '';
    this.successMessage = '';
    const isAdminRegistration = this.isAdmin() && formValue.accountRole === 'Admin';
    const registration = isAdminRegistration
      ? this.customerService.registerAdmin(this.registerData)
      : this.customerService.registerCustomer(this.registerData);

    registration.subscribe({
      next: () => {
        if (this.isAdmin()) {
          this.successMessage = isAdminRegistration
            ? 'Admin account created. They can now sign in.'
            : 'Customer account created successfully.';
          this.registerForm.reset({
            email: '',
            firstName: '',
            middleName: '',
            lastName: '',
            phone: '',
            dob: '',
            password: '',
            confirmPassword: '',
            accountRole: 'Customer'
          });
          return;
        }

        this.router.navigate(['login']);
      },
      error: (err) => {
        this.errorMessage = err?.error?.message || 'Registration failed.';
      }
    });
  }
}

