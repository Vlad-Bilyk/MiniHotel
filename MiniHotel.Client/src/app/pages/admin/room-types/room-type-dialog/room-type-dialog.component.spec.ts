import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RoomTypeDialogComponent } from './room-type-dialog.component';

describe('RoomTypeDialogComponent', () => {
  let component: RoomTypeDialogComponent;
  let fixture: ComponentFixture<RoomTypeDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [RoomTypeDialogComponent]
    })
      .compileComponents();

    fixture = TestBed.createComponent(RoomTypeDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
