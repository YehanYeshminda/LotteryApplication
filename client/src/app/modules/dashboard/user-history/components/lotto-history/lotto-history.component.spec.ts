import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LottoHistoryComponent } from './lotto-history.component';

describe('LottoHistoryComponent', () => {
  let component: LottoHistoryComponent;
  let fixture: ComponentFixture<LottoHistoryComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ LottoHistoryComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(LottoHistoryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
