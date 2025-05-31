import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UsersUpdateDialogComponent } from './users-update-dialog.component';

describe('UsersUpdateDialogComponent', () => {
  let component: UsersUpdateDialogComponent;
  let fixture: ComponentFixture<UsersUpdateDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [UsersUpdateDialogComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(UsersUpdateDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
