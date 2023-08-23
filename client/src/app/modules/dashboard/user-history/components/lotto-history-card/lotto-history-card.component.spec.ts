import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LottoHistoryCardComponent } from './lotto-history-card.component';

describe('LottoHistoryCardComponent', () => {
  let component: LottoHistoryCardComponent;
  let fixture: ComponentFixture<LottoHistoryCardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ LottoHistoryCardComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(LottoHistoryCardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
