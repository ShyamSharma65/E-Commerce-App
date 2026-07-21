import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Cutomerservice } from '../service/cutomerservice';
import { RegisterData } from '../Models/registerCustomer';

@Component({
  selector: 'app-register',
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './register.html',
  styleUrl: './register.css',
})
export class Register {

  registerData: RegisterData | null = null;

  constructor(private router: Router, private customerService: Cutomerservice) {

  }

  registerForm = new FormGroup({
    email: new FormControl('', [Validators.required]),
    firstName: new FormControl('', Validators.required),
    middleName: new FormControl(''),
    lastName: new FormControl('', Validators.required),
    phone: new FormControl('', Validators.required),
    dob: new FormControl('', Validators.required),
    password: new FormControl('', [Validators.required, Validators.maxLength(6), Validators.maxLength(20)]),
    confirmPassword: new FormControl('', [Validators.required, Validators.maxLength(6), Validators.maxLength(20)])
  })

  gotoLogin() {
    this.router.navigate(['login']);
  }

  register() {
    if (this.registerForm.valid) {
      const formValue = this.registerForm.value;

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
        next: (res) => {
          console.log(res);
        },
        error: (err) => {
          console.log(err);
        }
      })
    }
  }
}



