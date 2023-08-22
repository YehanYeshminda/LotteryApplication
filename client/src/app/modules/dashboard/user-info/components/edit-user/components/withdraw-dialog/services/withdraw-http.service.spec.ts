import { TestBed } from '@angular/core/testing';

import { WithdrawHttpService } from './withdraw-http.service';

describe('WithdrawHttpService', () => {
  let service: WithdrawHttpService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(WithdrawHttpService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
