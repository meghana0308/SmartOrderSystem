import { Component, EventEmitter, Output, OnInit, ChangeDetectorRef } from '@angular/core'
import { CommonModule } from '@angular/common'
import { NotificationService } from '../../core/services/notification-service'
import { NotificationDto } from '../../models/notification.model'

@Component({
  selector: 'app-notification-panel',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './notification-panel.html'
})
export class NotificationPanelComponent implements OnInit {

  notifications: NotificationDto[] = []
  @Output() close = new EventEmitter<void>()

  constructor(
    private service: NotificationService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit() {
    this.service.getMyNotifications().subscribe(res => {
      this.notifications = res
      this.cdr.detectChanges()
    })
  }

  markRead(n: NotificationDto) {
    this.service.markAsRead(n.id).subscribe(() => {
      n.isRead = true
      this.cdr.detectChanges()
    })
  }
}
