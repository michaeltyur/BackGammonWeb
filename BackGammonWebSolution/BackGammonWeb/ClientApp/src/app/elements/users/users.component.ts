import { Component, OnInit, OnDestroy } from '@angular/core';
import { UserService } from 'src/app/shared/services/user.service';
import { SignalRConnectionService } from 'src/app/shared/services/signal-r-connection.service';
import { ChatService } from 'src/app/shared/services/chat.service';
import { Subscription } from 'rxjs';
import { User } from 'src/app/shared/models/user';

@Component({
  selector: 'app-users',
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.scss']
})
export class UsersComponent implements OnInit, OnDestroy {
  subscription = new Subscription();
  usersOnLine: Array<User> = [];
  usersOffLine: Array<User> = [];
  constructor(
    private userService: UserService,
    private signalRConnectionService: SignalRConnectionService,
    private chatService: ChatService
  ) { }

  ngOnInit() {

    this.subscription.add(this.userService.users$.subscribe(res => {
      if (res) {
        this.setUsersArrays(res);
      }
    }));

    this.getAllUser();
  }

  ngOnDestroy() {
    this.subscription.unsubscribe();
  }
  getAllUser(): void {

    this.subscription.add(this.userService.getAllUser().subscribe(res => {
      if (res) {
        this.setUsersArrays(res);
      }
    }, error => {
      console.error(error);
    }));

  }
  setUsersArrays(allUsers: Array<User>): void {
    if (allUsers && allUsers.length) {

      this.usersOnLine = [];
      this.usersOffLine = [];

      allUsers.forEach(element => {
        if (element.isOnline) {
          this.usersOnLine.push(element);
        }
        else {
          this.usersOffLine.push(element);
        }
      });
    }

  }


}
