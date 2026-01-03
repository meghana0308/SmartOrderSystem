import { Component } from '@angular/core'
import { CommonModule } from '@angular/common'
import { RouterModule } from '@angular/router'
import { NavbarComponent } from '../../../layout/navbar/navbar'

@Component({
  selector: 'app-finance-shell',
  standalone: true,
  imports: [CommonModule, RouterModule, NavbarComponent],
  templateUrl: './finance-shell.html',
  styleUrls: ['./finance-shell.css']
})
export class FinanceShellComponent {}
