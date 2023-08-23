import { TestBed } from '@angular/core/testing';

import { LottoHistoryHttpService } from './lotto-history-http.service';

describe('LottoHistoryHttpService', () => {
  let service: LottoHistoryHttpService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(LottoHistoryHttpService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
