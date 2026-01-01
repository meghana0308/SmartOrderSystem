import { Component, Input } from '@angular/core';
import { OrderStatusPipe } from '../../pipes/order-status-pipe';

@Component({
  selector: 'app-status-badge',
  standalone: true,
  imports: [OrderStatusPipe],
  template: `<span class="badge">{{ status | orderStatus }}</span>`
})
export class StatusBadgeComponent {
  @Input() status!: string;
}
