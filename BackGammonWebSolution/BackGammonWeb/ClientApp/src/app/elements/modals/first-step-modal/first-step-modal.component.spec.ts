import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FirstStepModalComponent } from './first-step-modal.component';

describe('FirstStepModalComponent', () => {
  let component: FirstStepModalComponent;
  let fixture: ComponentFixture<FirstStepModalComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FirstStepModalComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FirstStepModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
