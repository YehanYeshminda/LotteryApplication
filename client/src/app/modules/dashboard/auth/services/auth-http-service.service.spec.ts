import { TestBed } from '@angular/core/testing';

import { AuthHttpServiceService } from './auth-http.service';

describe('AuthHttpServiceService', () => {
  let service: AuthHttpServiceService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(AuthHttpServiceService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
