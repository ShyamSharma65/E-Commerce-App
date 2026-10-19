import { Routes } from '@angular/router';
import { Login } from './customer/login/login';
import { Register } from './customer/register/register';
import { Home } from './customer/home/home';
import { AdminDashboard } from './admin/admin-dashboard';
import { adminGuard } from './admin/admin.guard';

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
        path: 'admin',
        component: AdminDashboard,
        canActivate: [adminGuard]
    },
    {
        path: '**',
        redirectTo: ''
    }
];
