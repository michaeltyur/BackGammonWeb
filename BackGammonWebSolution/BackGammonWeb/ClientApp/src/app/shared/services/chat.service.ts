import { Injectable, EventEmitter } from '@angular/core';
import { SignalRConnectionService } from './signal-r-connection.service';
import { HubConnection } from '@aspnet/signalr';
import { SendMessage, ISendMessage } from '../models/message-send';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { ChatMessage } from '../models/chat-message';
import { User } from '../models/user';
import { ChatInvitation } from '../models/chat-invitation';
import { ChatClosing } from '../models/chat-closing';

@Injectable({
  providedIn: 'root'
})
export class ChatService {

  users$ = new EventEmitter();
  message$ = new EventEmitter();
  invitationToChat$ = new EventEmitter<ChatInvitation>();
  switchToChat$ = new EventEmitter<ChatInvitation>();
  closePrivateChat$ = new EventEmitter<string>();
  privateChatClosedByOtherUser$ = new EventEmitter<ChatClosing>();
  constructor(
    private signalRConnectionService: SignalRConnectionService,
    private httpClient: HttpClient) {

    signalRConnectionService.isConnected$.subscribe(res => {
      if (res) {

        signalRConnectionService.connection.on("InviteToPrivateChat", (res:ChatInvitation) => {
          if (res) {
            this.invitationToChat$.emit(res);
          }
        });

        signalRConnectionService.connection.on("PrivateChatClosed", (res:ChatClosing) => {
          if (res) {
            this.privateChatClosedByOtherUser$.emit(res);
          }
        });
      }
    });

  }

  sendMessage(message: ISendMessage): Promise<any> {
    if (this.signalRConnectionService.connection) {
      return this.signalRConnectionService.connection.invoke("SendMessage", message);
    }
    else return Promise.reject('connection is null');
  }

  getNumberOfMessages(number: number): Observable<Array<ISendMessage>> {
    let url = "/api/chat/getPublicMessages?numberOfMessages=" + number;
    return this.httpClient.get<Array<ISendMessage>>(url);
  }

  getNumberOfMessagesOfPrivateChat(number: number,groupName): Observable<Array<ISendMessage>> {
    let url = `/api/chat/getPrivateMessages?numberOfMessages=number&groupName=groupName`;
    return this.httpClient.get<Array<ISendMessage>>(url);
  }

  convertToChatMsg(message: ISendMessage): ChatMessage {

    let user = localStorage.getItem("userName");
    let msg: ChatMessage = new ChatMessage();
    msg.user.name = message.userName ? message.userName : "Nan";
    msg.date = message.date ? new Date(message.date) : new Date(Date.now());
    msg.text = message.content ? message.content : "Nan";
    msg.reply = (message.userName !== user) ? true : false;

    return msg;

  }

  openPrivateChat(userID: number): Promise<any> {
    return this.signalRConnectionService.connection.invoke("AddToGroup", userID).then((res: ChatInvitation) => {
      this.switchToChat$.emit(res);
      return res;
    });
  }

  closePrivateChat(groupName: string): Promise<any> {

    return this.signalRConnectionService.connection.invoke("ClosePrivateChat", groupName);
   }


}
