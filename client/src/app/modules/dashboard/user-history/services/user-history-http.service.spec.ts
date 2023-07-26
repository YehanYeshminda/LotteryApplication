import { TestBed } from '@angular/core/testing';

import { UserHistoryHttpService } from './user-history-http.service';

describe('UserHistoryHttpService', () => {
  let service: UserHistoryHttpService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(UserHistoryHttpService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
