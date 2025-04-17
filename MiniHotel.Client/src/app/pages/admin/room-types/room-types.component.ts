import { Component, OnInit } from '@angular/core';
import { RoomTypeDto, RoomTypeUpsertDto } from '../../../api/models';
import { RoomTypesService } from '../../../api/services';
import { ToastrService } from 'ngx-toastr';
import {
  RoomTypeDialogComponent,
  RoomTypeFormData,
} from './room-type-dialog/room-type-dialog.component';
import { DialogService } from '../../../shared/services/dialog.service';

@Component({
  selector: 'app-room-types',
  standalone: false,
  templateUrl: './room-types.component.html',
  styleUrl: './room-types.component.css',
})
export class RoomTypesComponent implements OnInit {
  roomTypes: RoomTypeDto[] = [];
  isLoading = false;

  constructor(
    private roomTypesService: RoomTypesService,
    private dialogService: DialogService,
    private toastr: ToastrService
  ) { }

  ngOnInit(): void {
    this.fetchRoomTypes();
  }

  fetchRoomTypes(): void {
    this.isLoading = true;
    this.roomTypesService.getRoomTypes().subscribe({
      next: (data) => {
        this.roomTypes = data;
        this.isLoading = false;
      },
      error: (err) => {
        this.toastr.error('Виникла помилка при завантаженні даних');
        console.error(err);
        this.isLoading = false;
      },
    });
  }

  deleteRoomType(id: number): void {
    if (!confirm('Ви впевнені що хочете видалити цей запис?')) return;

    this.roomTypesService.deleteRoomType({ id }).subscribe({
      next: () => {
        this.toastr.success('Запис видалено');
        this.fetchRoomTypes();
      },
      error: (err) => {
        this.toastr.error('Сталася помилка під час видалення');
        console.error(err);
      },
    });
  }

  editRoomType(rt: RoomTypeDto): void {
    const dialogData: RoomTypeFormData = {
      isEdit: true,
      formData: {
        roomCategory: rt.roomCategory!,
        pricePerNight: rt.pricePerNight,
        description: rt.description,
      },
    };

    this.dialogService
      .openEntityDialog<RoomTypeFormData, RoomTypeUpsertDto>(
        RoomTypeDialogComponent,
        dialogData,
        (data) =>
          this.roomTypesService.updateRoomType({
            id: rt.roomTypeId!,
            body: data,
          }),
        'Запис успішно оновлено'
      )
      .subscribe(() => {
        this.fetchRoomTypes();
      });
  }

  createRoomType(): void {
    const dialogData: RoomTypeFormData = {
      isEdit: false,
      formData: undefined,
    };

    this.dialogService
      .openEntityDialog<RoomTypeFormData, RoomTypeUpsertDto>(
        RoomTypeDialogComponent,
        dialogData,
        (data) => this.roomTypesService.createRoomType({ body: data }),
        'Додано новий тип кімнати'
      )
      .subscribe(() => {
        this.fetchRoomTypes();
      });
  }
}
