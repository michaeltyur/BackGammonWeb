import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { BackgammonGameComponent } from './backgammon-game.component';

describe('BackgammonGameComponent', () => {
  let component: BackgammonGameComponent;
  let fixture: ComponentFixture<BackgammonGameComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ BackgammonGameComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BackgammonGameComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
