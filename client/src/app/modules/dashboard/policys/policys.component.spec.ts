import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PolicysComponent } from './policys.component';

describe('PolicysComponent', () => {
  let component: PolicysComponent;
  let fixture: ComponentFixture<PolicysComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PolicysComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PolicysComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
