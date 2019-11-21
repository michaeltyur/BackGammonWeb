import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { GameStackComponent } from './game-stack.component';

describe('GameStackComponent', () => {
  let component: GameStackComponent;
  let fixture: ComponentFixture<GameStackComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ GameStackComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(GameStackComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
