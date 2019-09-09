import { Component, OnInit, OnDestroy, Inject } from '@angular/core';
import { User } from 'src/app/shared/models/user';
import { Subscription } from 'rxjs';
import { UserService } from 'src/app/shared/services/user.service';
import { SignalRConnectionService } from 'src/app/shared/services/signal-r-connection.service';
import { ISendMessage } from 'src/app/shared/models/message-send';
import { ChatService } from 'src/app/shared/services/chat.service';
import { IDictionary, Dictionary } from 'src/app/shared/models/dictionary';
import { ChatMessage } from 'src/app/shared/models/chat-message';
import { DOCUMENT } from '@angular/common';
import { ChatInvitation } from 'src/app/shared/models/chat-invitation';
import { NbToastrService } from '@nebular/theme';

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
  groupName: string = "public";
  isChat: boolean = true;
  isMobile: boolean = false;

  constructor(
    private nbToastrService:NbToastrService,
    private userService: UserService,
    private chatService: ChatService,
    private signalRConnectionService: SignalRConnectionService
  ) {
    this.signalRConnectionService.startConnection();
  }

  ngOnInit() {

    if (window.innerWidth < 600) {
      this.isMobile = true;
    }

    this.signalRConnectionService.connection.on("BroadcastMessage", res => {
      if (res) {
        res = JSON.parse(res);
        let sendMessage=<ISendMessage>res;
        let msg = this.chatService.convertToChatMsg(sendMessage);
        let chat=this.allChatDictionary.getByKey(sendMessage.groupName)
        chat.push(msg);
      }
    });

    this.subscription.add(this.chatService.switchToChat$.subscribe(res => {
      if (res) {
        this.swichToChat(res.userName, res.groupName);
      }
    }));

    this.allChatDictionary = new Dictionary<ChatMessage>();

    this.getNumberOfMessages("public", 50);

  }

  ngOnDestroy() {
    this.subscription.unsubscribe();
  }

  getNumberOfMessages(key: string, numberOfMsgs: number): void {

    this.subscription
      .add(this.chatService.getNumberOfMessages(numberOfMsgs)
        .subscribe((res: Array<ISendMessage>) => {
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

  swichToChat(userName: string, groupName: string): void {
    if (groupName) {
      if (groupName === 'public') {
        this.groupName = "public";
        this.chatTitle = "Public Chat";
        this.nbToastrService.default('','switch to public chat');
      }
      else {
        this.groupName = groupName;
        this.chatTitle = "Chat with " + userName;
      }

      let chat = <ChatMessage[]>this.allChatDictionary.getByKey(groupName);
      if (chat) {
        this.currentChat = chat;
      }
      else {
        let chat = new Array<ChatMessage>();
        this.currentChat = chat;
        this.allChatDictionary.add(groupName, chat);
      }
    }
  }


}
