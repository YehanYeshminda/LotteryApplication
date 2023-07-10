import { TestBed } from '@angular/core/testing';

import { MegaDrawHttpService } from './mega-draw-http.service';

describe('MegaDrawHttpService', () => {
  let service: MegaDrawHttpService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(MegaDrawHttpService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
