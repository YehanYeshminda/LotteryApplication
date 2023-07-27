import { TestBed } from '@angular/core/testing';

import { HistoryHttpService } from './history-http.service';

describe('HistoryHttpService', () => {
  let service: HistoryHttpService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(HistoryHttpService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
