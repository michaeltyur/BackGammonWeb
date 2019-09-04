import { Component, HostListener } from '@angular/core';
import { SignalRConnectionService } from './shared/services/signal-r-connection.service';
import { AuthService } from './shared/services/auth.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  title = 'Backgammon-Chat';
  @HostListener('window:beforeunload', ['$event'])
  public beforeunloadHandler($event) {
  //$event.returnValue = "Are you sure?";
  this.authService.logout();
 }

  constructor(
    private signalRConnectionService:SignalRConnectionService,
    private authService:AuthService){

  }


}
