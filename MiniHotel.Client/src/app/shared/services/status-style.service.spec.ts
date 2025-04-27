import { TestBed } from '@angular/core/testing';

import { StatusStyleService } from './status-style.service';

describe('StatusStyleService', () => {
  let service: StatusStyleService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(StatusStyleService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
