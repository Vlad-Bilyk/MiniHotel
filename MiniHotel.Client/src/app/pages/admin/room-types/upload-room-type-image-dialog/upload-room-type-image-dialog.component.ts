import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { ToastrService } from 'ngx-toastr';
import { RoomTypesService } from '../../../../api/services';

@Component({
  selector: 'app-upload-room-type-image-dialog',
  standalone: false,
  templateUrl: './upload-room-type-image-dialog.component.html',
  styleUrl: './upload-room-type-image-dialog.component.scss',
})
export class UploadRoomTypeImageDialogComponent {
  selectedFile?: File;
  uploading = false;
  previewUrl?: string;

  constructor(
    @Inject(MAT_DIALOG_DATA) public roomTypeId: number,
    private dialogRef: MatDialogRef<UploadRoomTypeImageDialogComponent>,
    private roomTypesService: RoomTypesService,
    private toastr: ToastrService
  ) { }

  onFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    if (file) {
      this.selectedFile = file;
      this.generatedPrewiew(file);
    }
  }

  uploadImage() {
    if (!this.selectedFile) return;

    this.uploading = true;

    this.roomTypesService
      .uploadImage({
        id: this.roomTypeId,
        body: { file: this.selectedFile },
      })
      .subscribe({
        next: () => {
          this.toastr.success('Фото успішно завантажено');
          this.dialogRef.close(true);
        },
        error: (err) => {
          console.error(err);
          this.toastr.error('Помилка при завантаженні фото');
          this.uploading = false;
        },
      });
  }

  cancel() {
    this.dialogRef.close(false);
  }

  private generatedPrewiew(file: File): void {
    const reader = new FileReader();
    reader.onload = (e) => {
      this.previewUrl = e.target?.result as string;
    };
    reader.readAsDataURL(file);
  }
}
