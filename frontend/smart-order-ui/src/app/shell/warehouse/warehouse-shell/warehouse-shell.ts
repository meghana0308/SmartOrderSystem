import { Component } from '@angular/core'
import { CommonModule } from '@angular/common'
import { RouterOutlet, RouterLink } from '@angular/router'
@Component({
  selector: 'app-warehouse-shell',
  standalone: true,
  imports: [CommonModule,RouterLink , RouterOutlet],
  templateUrl: './warehouse-shell.html',
  styleUrl: './warehouse-shell.css'
})
export class WarehouseShellComponent {}
