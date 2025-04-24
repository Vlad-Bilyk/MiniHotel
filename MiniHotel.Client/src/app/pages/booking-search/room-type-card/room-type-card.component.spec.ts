import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RoomTypeCardComponent } from './room-type-card.component';

describe('RoomCardComponent', () => {
  let component: RoomTypeCardComponent;
  let fixture: ComponentFixture<RoomTypeCardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [RoomTypeCardComponent]
    })
      .compileComponents();

    fixture = TestBed.createComponent(RoomTypeCardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
