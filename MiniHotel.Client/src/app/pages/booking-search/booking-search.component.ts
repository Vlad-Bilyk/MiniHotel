import { Component, OnInit } from '@angular/core';
import {
  AbstractControl,
  FormBuilder,
  FormGroup,
  ValidatorFn,
  Validators,
} from '@angular/forms';
import { RoomDto } from '../../api/models';
import { RoomsService } from '../../api/services';
import { ToastrService } from 'ngx-toastr';

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
    private toastr: ToastrService
  ) { }

  ngOnInit(): void {
    this.searchForm = this.fb.group(
      {
        startDate: [this.defaultStartDate, Validators.required],
        endDate: [this.defaultEndDate, Validators.required],
      },
      { validators: this.dateRangeValidator }
    );
  }

  onSearch(): void {
    this.searchPerformed = false;

    if (this.searchForm.invalid) {
      this.searchForm.markAllAsTouched();
      this.toastr.error('Будь ласка, перевірте правильність введення дат.');
      return;
    }

    const { startDate, endDate } = this.searchForm.value;

    this.roomService.getAvailableRooms({ startDate, endDate }).subscribe({
      next: (rooms) => {
        this.searchPerformed = true;
        this.groupedRooms = this.groupByCategory(rooms);
        if (rooms.length === 0) {
          this.toastr.info(
            'Вибачте, на запитуваний період немає вільних номерів.'
          );
        }
      },
      error: (err) => {
        this.toastr.error('Не вдалося завантажити номери. Спробуйте пізніше');
        console.log(err);
      },
    });
  }

  onBook(room: RoomDto): void {
    // TODO: go to booking component
    console.log('Booked room:', room);
  }

  private groupByCategory(
    rooms: RoomDto[]
  ): Map<string, { rooms: RoomDto[]; total: number }> {
    const map = new Map<string, { rooms: RoomDto[]; total: number }>();
    for (const room of rooms) {
      const key = room.roomType;
      if (key === undefined) {
        this.toastr.error('Room type is undefined. Please contact support.');
        console.error(
          'Room type is undefined for room with ID: ' + room.roomId
        );
        continue;
      }

      if (!map.has(key)) {
        map.set(key, { rooms: [], total: 0 });
      }

      map.get(key)!.rooms.push(room);
      map.get(key)!.total++;
    }

    this.roomCategoriesData = Array.from(map.entries());

    return map;
  }

  private defaultStartDate(): string {
    return new Date().toISOString().split('T')[0];
  }

  private defaultEndDate(): string {
    const tommorow = new Date();
    tommorow.setDate(tommorow.getDate() + 1);
    return tommorow.toISOString().split('T')[0];
  }

  private dateRangeValidator: ValidatorFn = (group: AbstractControl) => {
    const start = group.get('startDate')?.value;
    const end = group.get('endDate')?.value;
    return start && end && start >= end ? { indvalidRange: true } : null;
  };
}
