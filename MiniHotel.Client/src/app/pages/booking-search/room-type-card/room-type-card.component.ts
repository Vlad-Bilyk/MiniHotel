import { Component, EventEmitter, Input, Output } from '@angular/core';
import { RoomTypeDto } from '../../../api/models';

@Component({
  selector: 'app-room-type-card',
  standalone: false,
  templateUrl: './room-type-card.component.html',
  styleUrl: './room-type-card.component.css'
})
export class RoomTypeCardComponent {
  @Input() roomType!: RoomTypeDto;
  @Input() categoryName!: string;
  @Input() totalAvailable: number = 0;

  @Output() book = new EventEmitter<RoomTypeDto>();

  bookRoom(): void {
    this.book.emit(this.roomType);
  }
}
