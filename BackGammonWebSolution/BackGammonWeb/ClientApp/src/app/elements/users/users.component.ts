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
  owner:string;
  constructor(
    private userService: UserService,
    private signalRConnectionService: SignalRConnectionService,
    private chatService: ChatService
  ) { }

  ngOnInit() {
    this.owner=localStorage.getItem("userName");
    this.subscription.add(this.userService.users$.subscribe(res => {
      if (res) {
        this.setUsersArrays(res);
      }
    }));

    this.chatService.users$.subscribe(res=>{
      if (res) {
        this.setUsersArrays(res);
      }
    })

    this.subscription.add(this.chatService.inviterToChat$.subscribe(res => {
      if (res) {
        this.openPrivateChatFromRemote(res);
      }
    }, error => console.error(error)));

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

  openPrivateChat(userName: string):void{
   if (userName) {
     this.userService.openPrivateChat(userName).then().catch(error=>console.error(error));
   }
  }

  openPrivateChatFromRemote(userName: string): void {
    if (userName) {
      let user = this.usersOnLine.find(el => el.userName === userName)
      if (user) {
        user.haveNewPrivateChat = true;
      }
    }
  }

}
