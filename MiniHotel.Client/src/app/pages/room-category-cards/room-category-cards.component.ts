import { Component, OnInit } from '@angular/core';
import { RoomTypesService } from '../../api/services';
import { ToastrService } from 'ngx-toastr';
import { RoomTypeDto } from '../../api/models';

@Component({
  selector: 'app-room-category-cards',
  standalone: false,
  templateUrl: './room-category-cards.component.html',
  styleUrl: './room-category-cards.component.css'
})
export class RoomCategoryCardsComponent implements OnInit {
  roomTypes: RoomTypeDto[] = [];
  loading = true;

  constructor(
    private roomTypesService: RoomTypesService,
    private toastr: ToastrService
  ) { }

  ngOnInit(): void {
    this.loadRoomTypes();
  }

  loadRoomTypes(): void {
    this.roomTypesService.getRoomTypes().subscribe({
      next: (data) => {
        this.roomTypes = data;
        this.loading = false;
      },
      error: (err) => {
        console.error(err);
        this.toastr.error('Не вдалося завантажити')
        this.loading = false;
      }
    })
  }
}
