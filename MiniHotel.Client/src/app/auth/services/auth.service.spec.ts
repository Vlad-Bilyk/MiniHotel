import { TestBed } from '@angular/core/testing';

import { AuthServiceWrapper } from './auth.service';

describe('AuthService', () => {
  let service: AuthServiceWrapper;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(AuthServiceWrapper);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
