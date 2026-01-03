import { Component } from '@angular/core'
import { CommonModule } from '@angular/common'
import { RouterOutlet, RouterLink } from '@angular/router'
import { NavbarComponent } from '../../../layout/navbar/navbar'

@Component({
  selector: 'app-warehouse-shell',
  standalone: true,
  imports: [CommonModule,RouterLink , RouterOutlet,NavbarComponent],
  templateUrl: './warehouse-shell.html',
  styleUrl: './warehouse-shell.css'
})
export class WarehouseShellComponent {}
