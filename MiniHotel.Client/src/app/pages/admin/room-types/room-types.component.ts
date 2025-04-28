import { Component, OnInit, ViewChild } from '@angular/core';
import { RoomTypeDto, RoomTypeUpsertDto } from '../../../api/models';
import { RoomTypesService } from '../../../api/services';
import { ToastrService } from 'ngx-toastr';
import {
  RoomTypeDialogComponent,
  RoomTypeFormData,
} from './room-type-dialog/room-type-dialog.component';
import { DialogService } from '../../../shared/services/dialog.service';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatDialog } from '@angular/material/dialog';
import { UploadRoomTypeImageDialogComponent } from './upload-room-type-image-dialog/upload-room-type-image-dialog.component';
import { ImagePreviewDialogComponent } from '../../../shared/dialogs/image-preview-dialog/image-preview-dialog.component';

@Component({
  selector: 'app-room-types',
  standalone: false,
  templateUrl: './room-types.component.html',
  styleUrl: './room-types.component.scss',
})
export class RoomTypesComponent implements OnInit {
  displayedColumns = ['category', 'price', 'image', 'description', 'actions'];

  dataSource = new MatTableDataSource<RoomTypeDto>([]);
  loading = false;
  searchValue = "";

  @ViewChild(MatPaginator) set MatPaginator(p: MatPaginator) {
    this.dataSource.paginator = p;
  }

  constructor(
    private roomTypesService: RoomTypesService,
    private dialogService: DialogService,
    private toastr: ToastrService,
    private dialog: MatDialog
  ) { }

  ngOnInit(): void {
    this.loadRoomTypes();
  }

  loadRoomTypes(): void {
    this.loading = true;
    this.roomTypesService.getRoomTypes().subscribe({
      next: (data) => {
        this.dataSource.data = data;
        this.loading = false;
      },
      error: (err) => {
        this.toastr.error('Виникла помилка при завантаженні даних');
        console.error(err);
        this.loading = false;
      },
    });
  }

  applyFilter(value: string) {
    this.dataSource.filter = value.trim().toLowerCase();
  }

  editRoomType(rt: RoomTypeDto): void {
    const dialogData: RoomTypeFormData = {
      isEdit: true,
      formData: {
        roomCategory: rt.roomCategory!,
        pricePerNight: rt.pricePerNight,
        description: rt.description!,
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
        'Запис успішно оновлено',
        '500px'
      )
      .subscribe(() => {
        this.loadRoomTypes();
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
        'Додано новий тип кімнати',
        '500px'
      )
      .subscribe(() => {
        this.loadRoomTypes();
      });
  }

  openUploadImageDialog(roomType: RoomTypeDto): void {
    const dialogRef = this.dialog.open(UploadRoomTypeImageDialogComponent, {
      width: '400px',
      data: roomType.roomTypeId
    });

    dialogRef.afterClosed().subscribe(success => {
      if (success) {
        this.loadRoomTypes();
      }
    });
  }

  openImagePreview(imageUrl: string): void {
    this.dialog.open(ImagePreviewDialogComponent, {
      data: { imageUrl: imageUrl },
      maxWidth: '90vw',
      maxHeight: '90vh',
      panelClass: 'image-preview-dialog'
    });
  }
}
