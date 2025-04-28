import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'app-image-preview-dialog',
  standalone: false,
  templateUrl: './image-preview-dialog.component.html',
  styleUrl: './image-preview-dialog.component.scss'
})
export class ImagePreviewDialogComponent {
  constructor(@Inject(MAT_DIALOG_DATA) public data: { imageUrl: string }) { }
}
