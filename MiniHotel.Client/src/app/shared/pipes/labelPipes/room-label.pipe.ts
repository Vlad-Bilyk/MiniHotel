import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'roomLabel',
  standalone: false
})
export class RoomLabelPipe implements PipeTransform {
  transform(value: number): string {
    if (value === 1) return 'кімната';
    if (value >= 2 && value <= 4) return 'кімнати';
    return 'кімнат';
  }
}
