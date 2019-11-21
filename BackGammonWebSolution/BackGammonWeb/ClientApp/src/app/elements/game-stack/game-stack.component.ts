import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'app-game-stack',
  templateUrl: './game-stack.component.html',
  styleUrls: ['./game-stack.component.scss']
})
export class GameStackComponent implements OnInit {
  @Input() checkersArray; 
  constructor() { }

  ngOnInit() {
  }

}
