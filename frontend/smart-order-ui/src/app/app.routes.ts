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
          import('./shell/admin/categories/categories').then((m) => m.CategoriesComponent),
      },
      {
        path: 'products',
        loadComponent: () =>
          import('./shell/admin/products/products').then((m) => m.ProductsComponent),
      },
      {
        path: 'users',
        loadComponent: () => import('./shell/admin/users/users').then((m) => m.UsersComponent),
        canActivate: [authGuard, roleGuard],
        data: { roles: ['Admin'] },
      },
      { path: '', redirectTo: 'categories', pathMatch: 'full' },
    ],
  },
  {
  path: 'sales',
  component: SalesShellComponent,
  canActivate: [authGuard, roleGuard],
  data: { roles: ['SalesExecutive'] },
  children: [
    {
      path: 'orders',
      loadComponent: () =>
        import('./shell/sales/orders/sales-orders/sales-orders')
          .then(m => m.SalesOrdersComponent)
    },
    {
      path: 'create-order',
      loadComponent: () =>
        import('./shell/sales/orders/sales-create-order/sales-create-order')
          .then(m => m.SalesCreateOrderComponent)
    },{
  path: 'reports',
  loadComponent: () =>
    import('./shell/sales/reports/sales-reports')
      .then(m => m.SalesReportsComponent)
},

    { path: '', redirectTo: 'orders', pathMatch: 'full' }
  ]
},
  {
  path: 'warehouse',
  component: WarehouseShellComponent,
  canActivate: [authGuard, roleGuard],
  data: { roles: ['WarehouseManager'] },
  children: [
    {
      path: 'orders',
      loadComponent: () =>
        import('./shell/warehouse/orders-status/warehouse-orders-status')
          .then(m => m.WarehouseOrdersComponent)
    },
    {
      path: 'inventory',
      loadComponent: () =>
        import('./shell/warehouse/inventory/warehouse-inventory')
          .then(m => m.WarehouseInventoryComponent)
    },
    {
      path: 'reports',
      loadComponent: () =>
        import('./shell/warehouse/reports/warehouse-reports')
          .then(m => m.WarehouseReportsComponent)
    },
    { path: '', redirectTo: 'orders', pathMatch: 'full' }
  ]
}
,
  {
  path: 'finance',
  component: FinanceShellComponent,
  canActivate: [authGuard, roleGuard],
  data: { roles: ['FinanceOfficer'] },
  children: [
    {
      path: 'invoices',
      loadComponent: () =>
        import('./shell/finance/invoices/finance-invoices')
          .then(m => m.FinanceInvoicesComponent),
    },{
  path: 'orders',
  loadComponent: () =>
    import('./shell/finance/invoices/finance-orders')
      .then(m => m.FinanceOrdersComponent),
},

    { path: '', redirectTo: 'invoices', pathMatch: 'full' },
  ],
},

  {
  path: 'customer',
  component: CustomerShellComponent,
  canActivate: [authGuard, roleGuard],
  data: { roles: ['Customer'] },
  children: [
  {
    path: 'orders',
    loadComponent: () =>
      import('./shell/customer/orders/customer-orders')
        .then(m => m.CustomerOrdersComponent)
  },
  {
    path: 'create-order',
    loadComponent: () =>
      import('./shell/customer/create-order/create-order')
        .then(m => m.CreateOrderComponent)
  },
  { path: '', redirectTo: 'orders', pathMatch: 'full' }
]

},

  { path: '**', redirectTo: '/login' },
];
