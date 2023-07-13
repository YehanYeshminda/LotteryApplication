import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SpashScreenComponent } from './spash-screen.component';

describe('SpashScreenComponent', () => {
  let component: SpashScreenComponent;
  let fixture: ComponentFixture<SpashScreenComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SpashScreenComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SpashScreenComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
