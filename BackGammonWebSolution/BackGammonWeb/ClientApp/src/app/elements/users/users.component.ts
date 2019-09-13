import { Component, OnInit, OnDestroy } from '@angular/core';
import { UserService } from 'src/app/shared/services/user.service';
import { SignalRConnectionService } from 'src/app/shared/services/signal-r-connection.service';
import { ChatService } from 'src/app/shared/services/chat.service';
import { Subscription } from 'rxjs';
import { User } from 'src/app/shared/models/user';
import { NbToastrService } from '@nebular/theme';
import { ChatInvitation } from 'src/app/shared/models/chat-invitation';

@Component({
  selector: 'app-users',
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.scss']
})
export class UsersComponent implements OnInit, OnDestroy {
  subscription = new Subscription();
  usersOnLine: Array<User> = [];
  usersOffLine: Array<User> = [];
  owner: string;
  loading: boolean = false;
  constructor(
    private nbToastrService: NbToastrService,
    private userService: UserService,
    private signalRConnectionService: SignalRConnectionService,
    private chatService: ChatService
  ) { }

  ngOnInit() {
    this.owner = localStorage.getItem("userName");
    this.subscription.add(this.userService.users$.subscribe(res => {
      if (res) {
        this.setUsersArrays(res);
      }
    }));

    this.chatService.users$.subscribe(res => {
      if (res) {
        this.setUsersArrays(res);
      }
    })

    this.subscription.add(this.chatService.invitationToChat$.subscribe(res => {
      if (res) {
        this.openPrivateChatFromRemote(res.userName, res.groupName);
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

  openPrivateChat(user: User): void {
    if (user) {

      if (user.userName === localStorage.getItem('userName')) {
        this.nbToastrService.warning('', 'Are you sure about that? You want to chat with your-self?');
        return;
      }
      else if (user.haveNewPrivateChat) {
        this.chatService.switchToChat$.emit({ userName: user.userName, groupName: user.groupName });
      }
      this.loading = true;
      this.chatService.openPrivateChat(user.userName).then((res: ChatInvitation) => {
        if (res) {
          this.loading = false;
          if (!res.error) {
            user.haveNewPrivateChat = true;
            this.chatService.switchToChat$.emit({ userName: res.invaterName, groupName: res.groupName });
            this.nbToastrService.default('', res.message);
          }
          else if (res.error) {
            this.nbToastrService.danger('', 'Error');
          }
        }
      }).catch(error => {
        console.error(error)
        this.loading = false;
        ;
      });
    }
  }



  openPrivateChatFromRemote(userName: string, groupName: string): void {
    if (userName) {
      let user = this.usersOnLine.find(el => el.userName === userName)
      if (user) {
        user.haveNewPrivateChat = true;
        user.groupName = groupName;
      }
    }
  }

  closePrivateChat(user: User): void {
    if (user) {
      this.chatService.closePrivateChat(user.groupName).then(res => {
        if (res) {
          this.chatService.closePrivateChat$.emit(user.groupName);
          user.haveNewPrivateChat = false
          this.nbToastrService.success('', 'Chat was close successfully');
        }
        else this.nbToastrService.danger('', 'Error');
      });
    }

  }

}
