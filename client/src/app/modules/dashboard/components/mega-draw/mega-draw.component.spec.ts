import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MegaDrawComponent } from './mega-draw.component';

describe('MegaDrawComponent', () => {
  let component: MegaDrawComponent;
  let fixture: ComponentFixture<MegaDrawComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MegaDrawComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MegaDrawComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
