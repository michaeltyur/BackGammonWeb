import { Component, OnInit, OnDestroy } from '@angular/core';
import { User } from 'src/app/shared/models/user';
import { Subscription } from 'rxjs';
import { UserService } from 'src/app/shared/services/user.service';
import { SignalRConnectionService } from 'src/app/shared/services/signal-r-connection.service';

@Component({
  selector: 'app-lobby',
  templateUrl: './lobby.component.html',
  styleUrls: ['./lobby.component.scss']
})
export class LobbyComponent implements OnInit, OnDestroy {
  subscription = new Subscription();
  usersOnLine: Array<User> = [];
  usersOffLine: Array<User> = [];
  isChat: boolean = true;
  constructor(private userService: UserService,private signalRConnectionService:SignalRConnectionService) { }

  ngOnInit() {
    this.signalRConnectionService.startConnection();
    this.getAllUser();
  }
  ngOnDestroy() {
    this.subscription.unsubscribe();
  }

  getAllUser(): void {
    this.userService.getAllUser().subscribe(res => {
      if (res) {
        res.forEach(element => {
          if (element.isOnline) {
            this.usersOnLine.push(element);
          }
          else {
            this.usersOffLine.push(element);
          }
        });
      }
    }, error => {
      console.error(error);
    })
  }

}
