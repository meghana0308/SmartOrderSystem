import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'orderStatus',
  standalone: true
})
export class OrderStatusPipe implements PipeTransform {
  transform(value: string): string {
    switch (value) {
      case 'P': return 'Pending';
      case 'C': return 'Completed';
      case 'X': return 'Cancelled';
      default: return value;
    }
  }
}
