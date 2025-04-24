import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditBookingDialogComponent } from './edit-booking-dialog.component';

describe('EditBookingDialogComponent', () => {
  let component: EditBookingDialogComponent;
  let fixture: ComponentFixture<EditBookingDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [EditBookingDialogComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EditBookingDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
