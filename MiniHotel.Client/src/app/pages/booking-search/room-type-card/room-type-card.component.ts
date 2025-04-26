import { Component, EventEmitter, Input, Output } from '@angular/core';
import { trigger, transition, style, animate } from '@angular/animations';
import { RoomDto, RoomTypeDto } from '../../../api/models';

@Component({
  selector: 'app-room-type-card',
  standalone: false,
  templateUrl: './room-type-card.component.html',
  styleUrl: './room-type-card.component.css',
  animations: [
    trigger('fadeIn', [
      transition(':enter', [
        style({ opacity: 0, transform: 'translateY(20px)' }),
        animate('400ms ease-out', style({ opacity: 1, transform: 'translateY(0)' }))
      ])
    ])
  ]
})
export class RoomTypeCardComponent {
  @Input() roomType!: RoomTypeDto;
  @Input() room!: RoomDto;
  @Input() totalAvailable: number = 0;

  @Output() book = new EventEmitter<RoomTypeDto>();

  bookRoom(): void {
    this.book.emit(this.room);
  }
}
