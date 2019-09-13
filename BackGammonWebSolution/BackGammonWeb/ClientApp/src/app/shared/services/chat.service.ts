import { Injectable, EventEmitter } from '@angular/core';
import { SignalRConnectionService } from './signal-r-connection.service';
import { HubConnection } from '@aspnet/signalr';
import { SendMessage, ISendMessage } from '../models/message-send';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { ChatMessage } from '../models/chat-message';
import { User } from '../models/user';
import { ChatInvitation } from '../models/chat-invitation';

@Injectable({
  providedIn: 'root'
})
export class ChatService {

  users$ = new EventEmitter();
  message$ = new EventEmitter();
  invitationToChat$ = new EventEmitter();
  switchToChat$ = new EventEmitter();
  closePrivateChat$ = new EventEmitter<string>();
  constructor(
    private signalRConnectionService: SignalRConnectionService,
    private httpClient: HttpClient) {

    signalRConnectionService.isConnected$.subscribe(res => {
      if (res) {

        // signalRConnectionService.connection.on("UpdateUsers", (res: Array<User>) => {
        //   if (res && res.length) {
        //     this.users$.emit(res);
        //   }
        // });

        signalRConnectionService.connection.on("InviteToPrivateChat", (res: ChatInvitation) => {
          if (res) {
            this.message$.emit({ userName: res.invaterName, groupName: res.groupName });
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

  convertToChatMsg(message: ISendMessage): ChatMessage {

    let user = localStorage.getItem("userName");
    let msg: ChatMessage = new ChatMessage();
    msg.user.name = message.userName ? message.userName : "Nan";
    msg.date = message.date ? new Date(message.date) : new Date(Date.now());
    msg.text = message.content ? message.content : "Nan";
    msg.reply = (message.userName !== user) ? true : false;

    return msg;

  }

  openPrivateChat(userName: string): Promise<any> {
    return this.signalRConnectionService.connection.invoke("AddToGroup", userName).then((res: ChatInvitation) => {
      this.switchToChat$.emit({ userName: res.invaterName, groupName: res.groupName });
      return res;
    });
  }

  closePrivateChat(groupName: string): Promise<any> {

    return this.signalRConnectionService.connection.invoke("ClosePrivateChat", groupName);
   }


}
