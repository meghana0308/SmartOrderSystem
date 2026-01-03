import { Component } from '@angular/core'
import { CommonModule } from '@angular/common'
import { RouterOutlet, RouterLink } from '@angular/router'
import { NavbarComponent } from '../../../layout/navbar/navbar'
@Component({
  selector: 'app-sales-shell',
  standalone: true,
  imports: [
    CommonModule,
    RouterOutlet,
    RouterLink,
     NavbarComponent
  ],
  templateUrl: './sales-shell.html',
  styleUrl: './sales-shell.css'
})
export class SalesShellComponent {}
