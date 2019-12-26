import { Component, HostListener, OnInit } from '@angular/core';
import { SignalRConnectionService } from './shared/services/signal-r-connection.service';
import { AuthService } from './shared/services/auth.service';
import { HttpClient } from '@angular/common/http';
import { UserService } from './shared/services/user.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  title = 'Backgammon-Chat';
  updateUserOnlineInterval;
//   @HostListener('window:beforeunload', ['$event'])
//   public beforeunloadHandler($event) {
//   $event.returnValue = "Are you sure?";
//   this.authService.logout();
//  }

  constructor(
    private signalRConnectionService:SignalRConnectionService,
    private authService:AuthService,
    private userService:UserService){

  }

  ngOnInit(){
   this.userService.checkIfUserLogged().subscribe(res=>{
     if (res) {
        this.userService.updateUserOnlineStart();
     }
     else{
      this.userService.updateUserOnlineStop();
     }
   })
  }
 

   
}
