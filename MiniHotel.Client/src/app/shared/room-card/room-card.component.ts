import { Component, EventEmitter, Input, Output } from '@angular/core';
import { RoomDto } from '../../api/models';

@Component({
  selector: 'app-room-card',
  standalone: false,
  templateUrl: './room-card.component.html',
  styleUrl: './room-card.component.css'
})
export class RoomCardComponent {
  @Input() room!: RoomDto;
  @Input() categoryName!: string;
  @Input() totalAvailable: number = 0;

  @Output() book = new EventEmitter<RoomDto>();

  bookRoom(): void {
    this.book.emit(this.room);
  }
}
