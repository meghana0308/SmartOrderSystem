import { Component } from '@angular/core'
import { CommonModule } from '@angular/common'
import { RouterModule } from '@angular/router'
import { AuthService } from '../../core/services/auth-service'
import { NotificationPanelComponent } from '../../shared/notification-panel/notification-panel'
@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, RouterModule,NotificationPanelComponent],
  templateUrl: './navbar.html'
})
export class NavbarComponent {

  role = ''
  showNotifications=false
  constructor(private auth: AuthService) {
    this.role = this.auth.getRole()
  }
   toggleNotifications(){
    this.showNotifications=!this.showNotifications
  }
  logout() {
    this.auth.logout()
    location.href = '/login'
  }
}
