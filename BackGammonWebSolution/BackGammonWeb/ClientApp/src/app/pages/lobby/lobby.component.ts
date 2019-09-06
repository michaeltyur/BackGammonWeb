import { Component, OnInit, OnDestroy, Inject } from '@angular/core';
import { User } from 'src/app/shared/models/user';
import { Subscription } from 'rxjs';
import { UserService } from 'src/app/shared/services/user.service';
import { SignalRConnectionService } from 'src/app/shared/services/signal-r-connection.service';
import { SendMessage } from 'src/app/shared/models/message-send';
import { ChatService } from 'src/app/shared/services/chat.service';
import { IDictionary, Dictionary } from 'src/app/shared/models/dictionary';
import { ChatMessage } from 'src/app/shared/models/chat-message';
import { DOCUMENT } from '@angular/common';

@Component({
  selector: 'app-lobby',
  templateUrl: './lobby.component.html',
  styleUrls: ['./lobby.component.scss']
})
export class LobbyComponent implements OnInit, OnDestroy {

  subscription = new Subscription();
  currentChat: Array<ChatMessage> = [];
  allChatDictionary: IDictionary;
  chatTitle: string = "Public Chat";
  isChat: boolean = true;
  isMobile: boolean = false;

  constructor(
    private userService: UserService,
    private signalRConnectionService: SignalRConnectionService,
    private chatService: ChatService,

  ) {
    this.signalRConnectionService.startConnection();
  }

  ngOnInit() {

    if (window.innerWidth < 600) {
      this.isMobile = true;
    }

    this.allChatDictionary = new Dictionary<ChatMessage>();

    this.getNumberOfMessages("public", 50);

  }

  ngOnDestroy() {
    this.subscription.unsubscribe();
  }

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
      this.signalRConnectionService.connection.invoke('AddToGroup', user.userName).then(res => {
        let chatArray = new Array<ChatMessage>();
      }).catch(err => console.error(err));
    }
  }



}
