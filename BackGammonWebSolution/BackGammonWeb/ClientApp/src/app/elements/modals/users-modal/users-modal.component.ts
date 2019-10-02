import { Component, OnInit, OnDestroy, Input } from '@angular/core';
import { UserService } from 'src/app/shared/services/user.service';
import { SignalRConnectionService } from 'src/app/shared/services/signal-r-connection.service';
import { ChatService } from 'src/app/shared/services/chat.service';
import { Subscription } from 'rxjs';
import { User } from 'src/app/shared/models/user';
import { NbToastrService } from '@nebular/theme';
import { ChatInvitation } from 'src/app/shared/models/chat-invitation';

@Component({
  selector: 'app-users-modal',
  templateUrl: './users-modal.component.html',
  styleUrls: ['./users-modal.component.scss']
})
export class UsersModalComponent implements OnInit, OnDestroy {
  subscription = new Subscription();
  usersOnLine: User[] = [];
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
    if (this.userService.users && this.userService.users.length) {
      this.setUsersArrays(this.userService.users);
    }
  }
  ngOnDestroy() {
    this.subscription.unsubscribe();
  }
  setUsersArrays(allUsers: Array<User>): void {
    if (allUsers && allUsers.length) {

      this.usersOnLine = [];

      allUsers.forEach(element => {
        if (element.isOnline) {
          this.usersOnLine.push(element);
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
        var chatInvitation: ChatInvitation = {
          inviterID: user.userID,
          groupName: user.groupName,
          message:null,
          error:null
        }
        this.chatService.switchToChat$.emit(chatInvitation);
      }
      this.loading = true;
      this.chatService.openPrivateChat(user.userID).then((res: ChatInvitation) => {
        if (res) {
          this.loading = false;
          if (!res.error) {
            user.haveNewPrivateChat = true;
            user.groupName = res.groupName;
            this.chatService.switchToChat$.emit(res);
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
