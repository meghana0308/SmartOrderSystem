import {
  Component,
  OnInit,
  ViewChild,
  Inject
} from '@angular/core'
import { CommonModule } from '@angular/common'
import { FormControl, ReactiveFormsModule } from '@angular/forms'

import { MatTableModule } from '@angular/material/table'
import { MatSelectModule } from '@angular/material/select'
import { MatOptionModule } from '@angular/material/core'
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner'
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator'
import { MatSort, MatSortModule } from '@angular/material/sort'
import { MatFormFieldModule } from '@angular/material/form-field'
import { MatInputModule } from '@angular/material/input'
import { MatDialog, MatDialogModule, MAT_DIALOG_DATA } from '@angular/material/dialog'
import { MatButtonModule } from '@angular/material/button'
import { MatTableDataSource } from '@angular/material/table'

import { AdminUserService } from '../../../core/services/user-service'
import { AdminUser } from '../../../models/admin-user.model'

@Component({
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatTableModule,
    MatSelectModule,
    MatOptionModule,
    MatProgressSpinnerModule,
    MatPaginatorModule,
    MatSortModule,
    MatFormFieldModule,
    MatInputModule,
    MatDialogModule,
    MatButtonModule
  ],
  template: `
<h2>Users & Role Assignment</h2>

<mat-form-field appearance="outline" style="width:300px;margin-bottom:15px">
  <mat-label>Search users</mat-label>
  <input matInput (keyup)="applyFilter($event)">
</mat-form-field>

<table
  mat-table
  matSort
  *ngIf="!loading"
  [dataSource]="dataSource"
  class="mat-elevation-z8">

  <ng-container matColumnDef="email">
    <th mat-header-cell *matHeaderCellDef mat-sort-header>Email</th>
    <td mat-cell *matCellDef="let u">{{ u.email }}</td>
  </ng-container>

  <ng-container matColumnDef="fullName">
    <th mat-header-cell *matHeaderCellDef mat-sort-header>Full Name</th>
    <td mat-cell *matCellDef="let u">{{ u.fullName }}</td>
  </ng-container>

  <ng-container matColumnDef="role">
    <th mat-header-cell *matHeaderCellDef mat-sort-header>Role</th>
    <td mat-cell *matCellDef="let u">
      <mat-select
        [formControl]="roleControls[u.userId]"
        [disabled]="u.role === 'Admin'"
        (selectionChange)="confirmRoleChange(u)">
        <mat-option *ngFor="let r of roles" [value]="r">
          {{ r }}
        </mat-option>
      </mat-select>
    </td>
  </ng-container>

  <tr mat-header-row *matHeaderRowDef="columns"></tr>
  <tr mat-row *matRowDef="let row; columns: columns"></tr>
</table>

<mat-paginator
  [pageSize]="5"
  [pageSizeOptions]="[5,10,20]"
  showFirstLastButtons>
</mat-paginator>

<div *ngIf="loading" style="margin-top:20px">
  <mat-spinner diameter="40"></mat-spinner>
</div>
`
})
export class UsersComponent implements OnInit {

  dataSource = new MatTableDataSource<AdminUser>()
  loading = false

  columns = ['email', 'fullName', 'role']

  roles = [
    'Admin',
    'Customer',
    'SalesExecutive',
    'WarehouseManager',
    'FinanceOfficer'
  ]

  roleControls: Record<string, FormControl<string>> = {}

  @ViewChild(MatPaginator) paginator!: MatPaginator
  @ViewChild(MatSort) sort!: MatSort

  constructor(
    private userService: AdminUserService,
    private dialog: MatDialog
  ) {}

  ngOnInit() {
    this.loadUsers()
  }

  loadUsers() {
    this.loading = true

    this.userService.getAllUsers().subscribe({
      next: users => {
        this.dataSource.data = users

        users.forEach(u => {
          this.roleControls[u.userId] =
            new FormControl(u.role, { nonNullable: true })
        })

        this.dataSource.paginator = this.paginator
        this.dataSource.sort = this.sort

        this.loading = false
      },
      error: () => {
        alert('Failed to load users')
        this.loading = false
      }
    })
  }

  applyFilter(event: Event) {
    const value = (event.target as HTMLInputElement).value
    this.dataSource.filter = value.trim().toLowerCase()
  }

  confirmRoleChange(user: AdminUser) {
    const control = this.roleControls[user.userId]
    const newRole = control.value
    const oldRole = user.role

    if (newRole === oldRole) return

    const dialogRef = this.dialog.open(RoleConfirmDialog, {
      width: '350px',
      data: { email: user.email, role: newRole }
    })

    dialogRef.afterClosed().subscribe(confirm => {
      if (!confirm) {
        control.setValue(oldRole, { emitEvent: false })
        return
      }

      this.userService.changeUserRole(user.userId, newRole).subscribe({
        next: () => user.role = newRole,
        error: err => {
          alert(err.error || 'Role change failed')
          control.setValue(oldRole, { emitEvent: false })
        }
      })
    })
  }
}

/* ===== Confirmation Dialog ===== */

@Component({
  standalone: true,
  imports: [MatDialogModule, MatButtonModule],
  template: `
<h3 mat-dialog-title>Confirm Role Change</h3>

<div mat-dialog-content>
  Change role of <b>{{ data.email }}</b> to <b>{{ data.role }}</b>?
</div>

<div mat-dialog-actions align="end">
  <button mat-button [mat-dialog-close]="false">Cancel</button>
  <button mat-raised-button color="primary" [mat-dialog-close]="true">
    Confirm
  </button>
</div>
`
})
export class RoleConfirmDialog {
  constructor(@Inject(MAT_DIALOG_DATA) public data: any) {}
}
