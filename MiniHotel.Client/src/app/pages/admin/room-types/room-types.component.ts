import { Component, OnInit } from '@angular/core';
import { RoomTypeDto, RoomTypeUpsertDto } from '../../../api/models';
import { RoomTypesService } from '../../../api/services';
import { ToastrService } from 'ngx-toastr';
import { MatDialog } from '@angular/material/dialog';
import {
  RoomTypeDialogComponent,
  RoomTypeFormData,
} from './room-type-dialog/room-type-dialog.component';
import { Observable } from 'rxjs';

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
    private toastr: ToastrService,
    private dialog: MatDialog
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
    console.log("EditRoomType method call: ",)
    this.openRoomTypeDialog(
      {
        isEdit: true,
        formData: {
          roomCategory: rt.roomCategory!,
          pricePerNight: rt.pricePerNight,
          description: rt.description,
        },
      },
      (data) =>
        this.roomTypesService.updateRoomType({
          id: rt.roomTypeId!,
          body: data,
        }),
      'Запис оновлено успішно'
    );
  }

  createRoomType(): void {
    this.openRoomTypeDialog(
      {
        isEdit: false,
        formData: undefined,
      },
      (data) => this.roomTypesService.createRoomType({ body: data }),
      'Додано новий тип кімнати'
    );
  }

  /**
   * Opens the dialog for both creating and editing a room type.
   * @param dialogData - Data to configure the dialog (edit flag and form data).
   * @param submitFn - The function to call when form is submitted.
   * @param successMessage - Message to display on successful operation.
   */
  private openRoomTypeDialog(
    dialogData: RoomTypeFormData,
    submitFn: (data: RoomTypeUpsertDto) => Observable<any>,
    successMeassage: string
  ): void {
    console.log("In openRoomTypeDialog")
    console.log(dialogData);

    const dialogRef = this.dialog.open(RoomTypeDialogComponent, {
      width: '400px',
      data: dialogData,
    });

    dialogRef
      .afterClosed()
      .subscribe((result: RoomTypeUpsertDto | undefined) => {
        if (result) {
          submitFn(result).subscribe({
            next: () => {
              this.toastr.success(successMeassage);
              this.fetchRoomTypes();
            },
            error: (err) => {
              this.toastr.error('Щось пішло не так');
              console.error(err);
            },
          });
        }
      });
  }
}
