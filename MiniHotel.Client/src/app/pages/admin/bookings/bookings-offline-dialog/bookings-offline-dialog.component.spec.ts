import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BookingsOfflineDialogComponent } from './bookings-offline-dialog.component';

describe('BookingsOfflineDialogComponent', () => {
  let component: BookingsOfflineDialogComponent;
  let fixture: ComponentFixture<BookingsOfflineDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [BookingsOfflineDialogComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(BookingsOfflineDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
