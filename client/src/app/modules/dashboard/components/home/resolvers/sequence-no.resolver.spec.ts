import { TestBed } from '@angular/core/testing';

import { SequenceNoResolver } from './sequence-no.resolver';

describe('SequenceNoResolver', () => {
  let resolver: SequenceNoResolver;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    resolver = TestBed.inject(SequenceNoResolver);
  });

  it('should be created', () => {
    expect(resolver).toBeTruthy();
  });
});
