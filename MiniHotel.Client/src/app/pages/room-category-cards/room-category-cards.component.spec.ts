import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RoomCategoryCardsComponent } from './room-category-cards.component';

describe('RoomCategoryCardsComponent', () => {
  let component: RoomCategoryCardsComponent;
  let fixture: ComponentFixture<RoomCategoryCardsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [RoomCategoryCardsComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(RoomCategoryCardsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
