import { Component, OnInit, ViewChild } from '@angular/core';
import { RoomDto, RoomStatus, RoomUpsertDto } from '../../../api/models';
import { RoomsService } from '../../../api/services';
import { ToastrService } from 'ngx-toastr';
import {
  RoomFormData,
  RoomFormDialogComponent,
} from './room-form-dialog/room-form-dialog.component';
import { DialogService } from '../../../shared/services/dialog.service';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';

@Component({
  selector: 'app-rooms',
  standalone: false,
  templateUrl: './rooms.component.html',
  styleUrl: './rooms.component.css',
})
export class RoomsComponent implements OnInit {
  displayedColumns = ['number', 'category', 'price', 'status', 'actions'];

  RoomStatus = RoomStatus;
  dataSource = new MatTableDataSource<RoomDto>([]);
  loading = true;
  searchValue = '';

  @ViewChild(MatPaginator) set MatPaginator(p: MatPaginator) {
    this.dataSource.paginator = p;
  }

  constructor(
    private roomsService: RoomsService,
    private toastr: ToastrService,
    private dialogService: DialogService
  ) { }

  ngOnInit(): void {
    this.loadRooms();
  }

  loadRooms(): void {
    this.loading = true;
    this.roomsService.getRooms().subscribe({
      next: (data) => {
        this.dataSource.data = data;
        this.loading = false;
      },
      error: (err) => {
        this.toastr.error('Сталася помилка під час завантаження');
        console.error(err);
      },
    });
  }

  applyFilter(value: string): void {
    this.dataSource.filter = value.trim().toLowerCase();
  }

  toggleRoomStatus(room: RoomDto): void {
    const action = room.roomStatus ? 'деактивувати' : 'активувати';

    if (!confirm(`Ви впевнені, що хочете ${action} цей номер?`)) return;

    const request$ = room.roomStatus === RoomStatus.Available
      ? this.roomsService.updateRoomStatus({ id: room.roomId!, newStatus: RoomStatus.UnderMaintenance })
      : this.roomsService.updateRoomStatus({ id: room.roomId!, newStatus: RoomStatus.Available });

    request$.subscribe({
      next: () => {
        this.toastr.success(`Номер успішно ${action}о`);
        this.loadRooms();
      },
      error: (err) => {
        this.toastr.error(`Сталася помилка під час спроби ${action} номер`);
        console.error(err);
      },
    });
  }

  editRoom(room: RoomDto): void {
    const dialogData: RoomFormData = {
      isEdit: true,
      formData: {
        roomNumber: room.roomNumber!,
        roomTypeId: room.roomTypeId!,
        roomStatus: room.roomStatus!,
      },
    };

    this.dialogService
      .openEntityDialog<RoomFormData, RoomUpsertDto>(
        RoomFormDialogComponent,
        dialogData,
        (data) =>
          this.roomsService.updateRoom({
            id: room.roomId!,
            body: data,
          }),
        'Запис успішно оновлено'
      )
      .subscribe(() => {
        this.loadRooms();
      });
  }

  createRoom(): void {
    const dialogData: RoomFormData = {
      isEdit: false,
      formData: undefined,
    };

    this.dialogService
      .openEntityDialog<RoomFormData, RoomUpsertDto>(
        RoomFormDialogComponent,
        dialogData,
        (data) => this.roomsService.createRoom({ body: data }),
        'Додано нову кімнату'
      )
      .subscribe(() => {
        this.loadRooms();
      });
  }
}
