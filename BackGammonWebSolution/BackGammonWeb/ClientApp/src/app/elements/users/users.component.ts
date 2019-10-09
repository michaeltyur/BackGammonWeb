import { Component, OnInit, OnDestroy, Input } from '@angular/core';
import { UserService } from 'src/app/shared/services/user.service';
import { SignalRConnectionService } from 'src/app/shared/services/signal-r-connection.service';
import { ChatService } from 'src/app/shared/services/chat.service';
import { Subscription } from 'rxjs';
import { User } from 'src/app/shared/models/user';
import { NbToastrService } from '@nebular/theme';
import { IChatInvitation } from 'src/app/shared/models/chat-invitation';
import { IPrivateChatByUser } from 'src/app/shared/models/private-chat-by-user';
import { ChatClosing } from 'src/app/shared/models/chat-closing';

@Component({
  selector: 'app-users',
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.scss']
})
export class UsersComponent implements OnInit, OnDestroy {
  subscription = new Subscription();
  usersOnLine: Array<User>=[];
  usersOffLine: Array<User>=[];
  userName: string;
  userID: number;
  loading: boolean = false;
  constructor(
    private nbToastrService: NbToastrService,
    private userService: UserService,
    private signalRConnectionService: SignalRConnectionService,
    private chatService: ChatService
  ) { }

  ngOnInit() {
    this.userName = localStorage.getItem("userName");
    this.userID = +localStorage.getItem("userID");

    if (!this.usersOnLine.length) {
      this.getAllUser();
    }

    this.subscription.add(this.userService.users$.subscribe(res => {
      if (res) {
        this.setUsersArrays(res);
      }
    }));

    this.subscription.add(this.chatService.users$.subscribe(res => {
      if (res) {
        this.setUsersArrays(res);
      }
    }));

    this.subscription.add(this.chatService.invitationToChat$.subscribe((res:IChatInvitation) => {
      if (res) {
        this.openPrivateChatFromRemote(res);
      }
    }, error => console.error(error)));

    this.subscription.add(this.chatService.privateChatClosedByOtherUser$.subscribe((res: ChatClosing) => {
      if (res) {
        this.closePrivateChatByRemote(res);
      }
    }, err => console.error(err)
    ));

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
        if (user.userID == userWithPrivateChat.opponentID && user.userName !== this.userName) {
          user.haveNewPrivateChat = true;
          user.groupName = userWithPrivateChat.groupName;
        }
        else if (user.userName !== this.userName) {
          user.haveNewPrivateChat = false;
          user.groupName = null;
        }
      });

    });
  }

  openPrivateChat(user: User): void {
    if (user) {
      this.loading = true;
      if (user.userName === localStorage.getItem('userName')) {
        this.nbToastrService.warning('', 'Are you sure about that? You want to chat with your-self?');
        return;
      }
      else if (user.haveNewPrivateChat) {
        var chatInvitation: IChatInvitation = {
          inviterID: user.userID,
          inviterName:user.userName,
          groupName: user.groupName,
          message: null,
          error: null
        }
        this.nbToastrService.default('', `switch to private chat with ${user.userName}`);
        this.chatService.switchToChat$.emit(chatInvitation);
        this.loading = false;
      }
      else {
        this.chatService.openPrivateChat(user.userID).then((res: IChatInvitation) => {
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

  openPrivateChatFromRemote(chatInvitation:IChatInvitation): void {
    if (chatInvitation) {
      let user = this.usersOnLine.find(el => el.userID === chatInvitation.inviterID);
      chatInvitation.inviterName=user.userName;
      if (user) {
        user.haveNewPrivateChat = true;
        user.groupName = chatInvitation.groupName;
      }
    }
    else console.error("chatInvitation is null");
  }
  closePrivateChatByRemote(chatClosing: ChatClosing): void {
    if (!chatClosing.closerID) {
      console.error("userName is null");
      return;
    }
    if (!chatClosing.groupName) {
      console.error("groupName is null");
      return;
    }
    let user = this.usersOnLine.find(user => user.userID === chatClosing.closerID);
    if (user) {
      user.haveNewPrivateChat = false;
    }
  }

  getAllUser(): void {

    this.subscription.add(this.userService.getAllUser().subscribe(res => {
      if (res) {
        this.setUsersArrays(res);
        this.subscription.add(this.chatService.getPrivateChatsByUserID(this.userID).subscribe((res:Array<IPrivateChatByUser>) => {
          if (res.length) {
            this.updatePrivateChatByUser(res);
          }
        }, error => console.error(error)));
      }
    }, error => {
      console.error(error);
    }));

  }



}
