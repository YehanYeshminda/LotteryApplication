import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EasyDrawComponent } from './easy-draw.component';

describe('EasyDrawComponent', () => {
  let component: EasyDrawComponent;
  let fixture: ComponentFixture<EasyDrawComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ EasyDrawComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EasyDrawComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
