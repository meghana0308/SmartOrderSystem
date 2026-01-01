import { Routes } from '@angular/router';
import { LoginComponent } from './auth/login/login';
import { RegisterComponent } from './auth/register/register';
import { authGuard } from './core/guards/auth-guard';
import { roleGuard, roleChildGuard } from './core/guards/role-guard';
import { AdminShellComponent } from './shell/admin/admin-shell/admin-shell';
import { CustomerShellComponent } from './shell/customer/customer-shell/customer-shell';
import { FinanceShellComponent } from './shell/finance/finance-shell/finance-shell';
import { SalesShellComponent } from './shell/sales/sales-shell/sales-shell';
import { WarehouseShellComponent } from './shell/warehouse/warehouse-shell/warehouse-shell';

export const appRoutes: Routes = [
  { path: '', redirectTo: '/login', pathMatch: 'full' },

  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  {
  path: 'admin',
  component: AdminShellComponent,
  canActivate: [authGuard, roleGuard],
  data: { roles: ['Admin'] },
  children: [
    {
      path: 'categories',
      loadComponent: () =>
        import('./shell/admin/categories/categories').then(m => m.CategoriesComponent)
    },
    {
      path: 'products',
      loadComponent: () =>
        import('./shell/admin/products/products').then(m => m.ProductsComponent)
    },
    {
      path: 'users',
      loadComponent: () =>
        import('./shell/admin/users/users').then(m => m.UsersComponent)
    },
    { path: '', redirectTo: 'categories', pathMatch: 'full' }
  ]
}
,
  {
    path: 'sales',
    component: SalesShellComponent,
    canActivate: [authGuard, roleGuard],
    data: { roles: ['SalesExecutive'] },
  },
  {
    path: 'warehouse',
    component: WarehouseShellComponent,
    canActivate: [authGuard, roleGuard],
    data: { roles: ['WarehouseManager'] },
  },
  {
    path: 'finance',
    component: FinanceShellComponent,
    canActivate: [authGuard, roleGuard],
    data: { roles: ['FinanceOfficer'] },
  },
  {
    path: 'customer',
    component: CustomerShellComponent,
    canActivate: [authGuard, roleGuard],
    data: { roles: ['Customer'] },
  },

  { path: '**', redirectTo: '/login' },
];
