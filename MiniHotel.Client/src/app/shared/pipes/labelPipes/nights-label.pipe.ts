import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'nightsLabel',
  standalone: false,
})
export class NightsLabelPipe implements PipeTransform {
  transform(value: number): string {
    if (value === 1) return 'ніч';
    if (value >= 2 && value <= 4) return 'ночі';
    return 'ночей';
  }
}
