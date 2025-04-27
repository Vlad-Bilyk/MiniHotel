import { Pipe, PipeTransform } from '@angular/core';
import { RoomStatus } from '../../../../api/models';

@Pipe({
  name: 'roomStatus',
  standalone: false
})
export class RoomStatusPipe implements PipeTransform {

  transform(value: RoomStatus | string): string {
    switch (value) {
      case RoomStatus.Available:
        return 'Доступний';
      case RoomStatus.UnderMaintenance:
        return 'На обслуговуванні';
      default:
        return value;
    }
  }

}
