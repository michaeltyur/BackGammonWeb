import { Component, OnInit, OnDestroy } from '@angular/core';
import { User } from 'src/app/shared/models/user';
import { Subscription } from 'rxjs';
import { UserService } from 'src/app/shared/services/user.service';
import { SignalRConnectionService } from 'src/app/shared/services/signal-r-connection.service';
import { SendMessage } from 'src/app/shared/models/message-send';
import { ChatService } from 'src/app/shared/services/chat.service';
import { IDictionary, Dictionary } from 'src/app/shared/models/dictionary';
import { ChatMessage } from 'src/app/shared/models/chat-message';

@Component({
  selector: 'app-lobby',
  templateUrl: './lobby.component.html',
  styleUrls: ['./lobby.component.scss']
})
export class LobbyComponent implements OnInit, OnDestroy {

  subscription = new Subscription();
  // usersOnLine: Array<User> = [];
  // usersOffLine: Array<User> = [];
  currentChat: Array<ChatMessage> = [];
  allChatDictionary: IDictionary;
  chatTitle: string = "Public Chat";
  isChat: boolean = true;

  constructor(
    private userService: UserService,
    private signalRConnectionService: SignalRConnectionService,
    private chatService: ChatService
    ) {
    this.signalRConnectionService.startConnection();
  }

  ngOnInit() {

    // this.subscription.add(this.userService.users$.subscribe(res => {
    //   if (res) {
    //     this.setUsersArrays(res);
    //   }
    // }));

    this.allChatDictionary = new Dictionary<ChatMessage>();
    this.getNumberOfMessages("public", 50);
    //this.getAllUser();

  }

  ngOnDestroy() {
    this.subscription.unsubscribe();
  }

  // getAllUser(): void {

  //   this.subscription.add(this.userService.getAllUser().subscribe(res => {
  //     if (res) {
  //       this.setUsersArrays(res);
  //     }
  //   }, error => {
  //     console.error(error);
  //   }));

  // }

  // setUsersArrays(allUsers: Array<User>): void {
  //   if (allUsers && allUsers.length) {

  //     this.usersOnLine = [];
  //     this.usersOffLine = [];

  //     allUsers.forEach(element => {
  //       if (element.isOnline) {
  //         this.usersOnLine.push(element);
  //       }
  //       else {
  //         this.usersOffLine.push(element);
  //       }
  //     });
  //   }

  // }

  getNumberOfMessages(key: string, numberOfMsgs: number): void {

    this.subscription
      .add(this.chatService.getNumberOfMessages(numberOfMsgs)
        .subscribe((res: Array<SendMessage>) => {
          if (res && res.length) {

            let chatArray = new Array<ChatMessage>();

            res.forEach(element => {
              chatArray.push(this.chatService.convertToChatMsg(element));
            });

            this.currentChat = chatArray;

            this.allChatDictionary.add(key, chatArray);
          }
        }, err => console.error(err)));

  }

  openPrivateChat(user: User): void {
    if (user && this.signalRConnectionService.connection) {
      this.signalRConnectionService.connection.invoke('AddToGroup',user.userName).then(res=>{
        let chatArray = new Array<ChatMessage>();
      }).catch(err=>console.error(err));
    }
  }

}
