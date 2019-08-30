import { Component } from '@angular/core';
import { SignalRConnectionService } from './shared/services/signal-r-connection.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  title = 'Backgammon-Chat';
  constructor(private signalRConnectionService:SignalRConnectionService){

  }
}
