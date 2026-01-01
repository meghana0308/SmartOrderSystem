import { Component } from '@angular/core'
import { CommonModule } from '@angular/common'
import { NavbarComponent } from '../../../layout/navbar/navbar'
import { RouterModule } from '@angular/router'

@Component({
  selector: 'app-admin-shell',
  standalone: true,
  imports: [CommonModule, NavbarComponent, RouterModule],
  template: `
    <app-navbar></app-navbar>

    <div style="padding:16px">
      <router-outlet></router-outlet>
    </div>
  `
})
export class AdminShellComponent {}
