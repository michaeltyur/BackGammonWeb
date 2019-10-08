import { Component, OnInit, OnDestroy, Input } from '@angular/core';
import { UserService } from 'src/app/shared/services/user.service';
import { SignalRConnectionService } from 'src/app/shared/services/signal-r-connection.service';
import { ChatService } from 'src/app/shared/services/chat.service';
import { Subscription } from 'rxjs';
import { User } from 'src/app/shared/models/user';
import { NbToastrService } from '@nebular/theme';
import { ChatInvitation } from 'src/app/shared/models/chat-invitation';
import { IPrivateChatByUser } from 'src/app/shared/models/private-chat-by-user';

@Component({
  selector: 'app-users',
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.scss']
})
export class UsersComponent implements OnInit, OnDestroy {
  subscription = new Subscription();
  @Input() usersOnLine: Array<User>;
  @Input() usersOffLine: Array<User>;
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

    this.subscription.add(this.chatService.invitationToChat$.subscribe(res => {
      // this.privateChatOpenFromRemote(res.inviterID);
    }, (error: any) => console.error(error)));

    this.subscription.add(this.userService.privateChatByUser$.subscribe((res: Array<IPrivateChatByUser>) => {
      if (res && res.length) {
        this.updatePrivateChatByUser(res);
      }

    }, (error: any) => console.error(error)));

  }

  ngOnDestroy() {
    this.subscription.unsubscribe();
  }

  updatePrivateChatByUser(array: Array<IPrivateChatByUser>): void {
    array.forEach(userWithPrivateChat => {
      this.usersOnLine.forEach(user => {
        if (user.userID == userWithPrivateChat.opponentID && user.userName !== this.owner) {
          user.haveNewPrivateChat = true;
          user.groupName = userWithPrivateChat.groupName;
        }
        else if (user.userName !== this.owner) {
          user.haveNewPrivateChat = false;
          user.groupName = null;
        }
      });

    });
  }

  // privateChatOpenFromRemote(inviterID: number): void {
  //   this.usersOnLine.find(u => u.userID === inviterID).haveNewPrivateChat = true;
  // }

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
      this.loading = true;
      if (user.userName === localStorage.getItem('userName')) {
        this.nbToastrService.warning('', 'Are you sure about that? You want to chat with your-self?');
        return;
      }
      else if (user.haveNewPrivateChat) {
        var chatInvitation: ChatInvitation = {
          inviterID: user.userID,
          groupName: user.groupName,
          message: null,
          error: null
        }
        this.nbToastrService.info('', `switch to private chat with ${user.userName}`);
        this.chatService.switchToChat$.emit(chatInvitation);
        this.loading = false;
      }
      else {
        this.chatService.openPrivateChat(user.userID).then((res: ChatInvitation) => {
          if (res) {
            this.loading = false;
            if (!res.error) {
              // user.haveNewPrivateChat = true;
              user.groupName = res.groupName;
              this.chatService.switchToChat$.emit(res);
              this.nbToastrService.default('', res.message);
            }
            else if (res.error) {
              this.nbToastrService.danger('', 'Error');
              this.loading = false;
            }
          }
        }).catch(error => {
          console.error(error)
          this.loading = false;
          ;
        });
      }


    }
  }

  closePrivateChat(user: User): void {
    if (user) {
      this.chatService.closePrivateChat(user.groupName).then(res => {
        if (res) {
          this.chatService.closePrivateChat$.emit(user.groupName);
          // user.haveNewPrivateChat = false
          this.nbToastrService.success('', 'Chat was close successfully');
        }
        else this.nbToastrService.danger('', 'Error');
      });
    }

  }

}
