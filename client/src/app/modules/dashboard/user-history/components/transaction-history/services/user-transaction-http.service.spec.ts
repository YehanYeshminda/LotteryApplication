import { TestBed } from '@angular/core/testing';

import { UserTransactionHttpService } from './user-transaction-http.service';

describe('UserTransactionHttpService', () => {
  let service: UserTransactionHttpService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(UserTransactionHttpService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
