import { Component, OnInit } from '@angular/core';
import { RoomDto } from '../../api/models';
import { RoomsService } from '../../api/services';

@Component({
  selector: 'app-rooms',
  standalone: false,
  templateUrl: './rooms.component.html',
  styleUrl: './rooms.component.css',
})
export class RoomsComponent implements OnInit {
  rooms: RoomDto[] = [];
  isLoading = true;

  constructor(private roomsService: RoomsService) { }

  ngOnInit(): void {
    this.roomsService.getRooms().subscribe({
      next: (data) => {
        this.rooms = data;
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Error loading roooms', err);
        this.isLoading = false;
      },
    });
  }
}
