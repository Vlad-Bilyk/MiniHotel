import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UploadRoomTypeImageDialogComponent } from './upload-room-type-image-dialog.component';

describe('UploadRoomTypeImageDialogComponent', () => {
  let component: UploadRoomTypeImageDialogComponent;
  let fixture: ComponentFixture<UploadRoomTypeImageDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [UploadRoomTypeImageDialogComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(UploadRoomTypeImageDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
