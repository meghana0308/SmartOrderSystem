import { Component } from '@angular/core'
import { CommonModule } from '@angular/common'
import { RouterOutlet, RouterLink } from '@angular/router'

@Component({
  selector: 'app-sales-shell',
  standalone: true,
  imports: [
    CommonModule,
    RouterOutlet,
    RouterLink
  ],
  templateUrl: './sales-shell.html',
  styleUrl: './sales-shell.css'
})
export class SalesShellComponent {}
