import { Routes } from '@angular/router';
import { Login } from './customer/login/login';
import { Register } from './customer/register/register';
import { Home } from './customer/home/home';

export const routes: Routes = [
    {
        path: '',
        component: Home
    },
    {
        path: 'login',
        component: Login
    },
    {
        path: 'register',
        component: Register
    },
    {
        path: '**',
        redirectTo: ''
    }
];
