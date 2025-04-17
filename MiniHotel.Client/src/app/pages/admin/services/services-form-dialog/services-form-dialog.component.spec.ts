import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ServicesFormDialogComponent } from './services-form-dialog.component';

describe('ServicesFormDialogComponent', () => {
  let component: ServicesFormDialogComponent;
  let fixture: ComponentFixture<ServicesFormDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ServicesFormDialogComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ServicesFormDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
