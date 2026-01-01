import { Component } from '@angular/core'
import { CommonModule } from '@angular/common'
import { RouterModule } from '@angular/router'
import { AuthService } from '../../core/services/auth-service'

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './navbar.html'
})
export class NavbarComponent {

  role = ''

  constructor(private auth: AuthService) {
    this.role = this.auth.getRole()
  }

  logout() {
    this.auth.logout()
    location.href = '/login'
  }
}
