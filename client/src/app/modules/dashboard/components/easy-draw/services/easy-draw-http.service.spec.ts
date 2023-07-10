import { TestBed } from '@angular/core/testing';

import { EasyDrawHttpService } from './easy-draw-http.service';

describe('EasyDrawHttpService', () => {
  let service: EasyDrawHttpService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(EasyDrawHttpService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
