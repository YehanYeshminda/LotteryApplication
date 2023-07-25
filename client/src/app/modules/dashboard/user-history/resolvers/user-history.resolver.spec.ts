import { TestBed } from '@angular/core/testing';

import { UserHistoryResolver } from './user-history.resolver';

describe('UserHistoryResolver', () => {
  let resolver: UserHistoryResolver;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    resolver = TestBed.inject(UserHistoryResolver);
  });

  it('should be created', () => {
    expect(resolver).toBeTruthy();
  });
});
