import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TransactionLosingCardComponent } from './transaction-losing-card.component';

describe('TransactionLosingCardComponent', () => {
  let component: TransactionLosingCardComponent;
  let fixture: ComponentFixture<TransactionLosingCardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ TransactionLosingCardComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TransactionLosingCardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
