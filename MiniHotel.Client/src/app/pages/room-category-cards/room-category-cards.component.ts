import { AfterViewInit, Component, OnInit } from '@angular/core';
import { RoomTypesService } from '../../api/services';
import { ToastrService } from 'ngx-toastr';
import { RoomTypeDto } from '../../api/models';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-room-category-cards',
  standalone: false,
  templateUrl: './room-category-cards.component.html',
  styleUrl: './room-category-cards.component.css',
})
export class RoomCategoryCardsComponent implements OnInit, AfterViewInit {
  roomTypes: RoomTypeDto[] = [];
  loading = true;

  private targetRoomTypeId: number | null = null;

  constructor(
    private roomTypesService: RoomTypesService,
    private toastr: ToastrService,
    private route: ActivatedRoute,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.route.paramMap.subscribe((params) => {
      const idParam = params.get('roomTypeId');
      if (idParam) {
        this.targetRoomTypeId = +idParam;
      }
    });

    this.loadRoomTypes();
  }

  ngAfterViewInit(): void {
    if (this.targetRoomTypeId !== null) {
      // We delay a little so that all the cards are accurately drawn
      setTimeout(() => {
        const element = document.getElementById(
          `room-${this.targetRoomTypeId}`
        );
        if (element) {
          element.scrollIntoView({ behavior: 'smooth', block: 'center' });
        }
      }, 300);
    }
  }

  loadRoomTypes(): void {
    this.roomTypesService.getRoomTypes().subscribe({
      next: (data) => {
        this.roomTypes = data.filter(rt => (rt.roomCount ?? 0) > 0);
        this.loading = false;
      },
      error: (err) => {
        console.error(err);
        this.toastr.error('Не вдалося завантажити');
        this.loading = false;
      },
    });
  }

  onBook(): void {
    this.router.navigate(['/booking-search']);
  }
}
