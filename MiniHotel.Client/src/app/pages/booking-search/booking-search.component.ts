import { Component, OnInit } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  Validators,
} from '@angular/forms';
import { RoomDto } from '../../api/models';
import { RoomsService } from '../../api/services';
import { ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';
import { dateRangeValidator } from '../../shared/validators/date-range.validator';

@Component({
  selector: 'app-booking-search',
  standalone: false,
  templateUrl: './booking-search.component.html',
  styleUrl: './booking-search.component.css',
})
export class BookingSearchComponent implements OnInit {
  searchForm!: FormGroup;
  groupedRooms: Map<string, { rooms: RoomDto[]; total: number }> = new Map();
  searchPerformed = false;
  roomCategoriesData: Array<[string, { rooms: RoomDto[]; total: number }]> = [];

  constructor(
    private fb: FormBuilder,
    private roomService: RoomsService,
    private toastr: ToastrService,
    private router: Router
  ) { }

  ngOnInit(): void {
    const today = new Date();
    const tomorrow = new Date();
    tomorrow.setDate(tomorrow.getDate() + 1);

    this.searchForm = this.fb.group(
      {
        startDate: [today, Validators.required],
        endDate: [tomorrow, Validators.required],
      },
      { validators: dateRangeValidator }
    );
  }

  onSearch(): void {
    this.searchPerformed = false;
    this.groupedRooms.clear();
    this.roomCategoriesData = [];

    if (this.searchForm.invalid) {
      this.searchForm.markAllAsTouched();
      this.toastr.error('Будь ласка, перевірте правильність введення дат.');
      return;
    }

    const { startDate, endDate } = this.searchForm.value;
    const formattedStartDate = startDate.toISOString().split('T')[0];
    const formattedEndDate = endDate.toISOString().split('T')[0];

    this.roomService.getAvailableRooms({ startDate: formattedStartDate, endDate: formattedEndDate }).subscribe({
      next: (rooms) => {
        this.searchPerformed = true;
        if (rooms.length === 0) {
          this.toastr.info(
            'Вибачте, на запитуваний період немає вільних номерів.'
          );
          return;
        }
        this.groupedRooms = this.groupByCategory(rooms);
        this.roomCategoriesData = Array.from(this.groupedRooms.entries());
      },
      error: (err) => {
        this.toastr.error('Не вдалося завантажити номери. Спробуйте пізніше');
        this.searchPerformed = true;
        console.log(err);
      },
    });
  }

  onBook(room: RoomDto): void {
    const { startDate, endDate } = this.searchForm.value;

    const stateStore = {
      roomType: room.roomCategory,
      roomNumber: room.roomNumber,
      pricePerDay: room.pricePerDay,
      startDate,
      endDate,
    };

    localStorage.setItem('bookingData', JSON.stringify(stateStore));
    this.router.navigate(['/booking-confirmation']);
  }

  private groupByCategory(
    rooms: RoomDto[]
  ): Map<string, { rooms: RoomDto[]; total: number }> {
    const map = new Map<string, { rooms: RoomDto[]; total: number }>();
    for (const room of rooms) {
      const key = room.roomCategory!;
      if (!key) {
        console.error(`Room with ID ${room.roomId} has no category`);
        continue;
      }
      if (!map.has(key)) {
        map.set(key, { rooms: [], total: 0 });
      }
      map.get(key)!.rooms.push(room);
      map.get(key)!.total++;
    }
    return map;
  }
}
