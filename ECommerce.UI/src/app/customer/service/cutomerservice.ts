import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { RegisterData } from '../Models/registerCustomer';

@Injectable({
  providedIn: 'root',
})
export class Cutomerservice {

  constructor(private http: HttpClient){

  }
  
   private url = 'https://localhost:7287/customer/registercustomer';

  registerCustomer(data : RegisterData):Observable<any>{
    return this.http.post<any>(this.url, data);
  }
}
