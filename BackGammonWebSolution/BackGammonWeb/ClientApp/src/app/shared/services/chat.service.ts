import { Injectable, EventEmitter } from '@angular/core';
import { SignalRConnectionService } from './signal-r-connection.service';
import { HubConnection } from '@aspnet/signalr';
import { SendMessage } from '../models/message-send';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { ChatMessage } from '../models/chat-message';

@Injectable({
  providedIn: 'root'
})
export class ChatService {


  message$ = new EventEmitter();
  inviterToChat$ = new EventEmitter();
  private hubConnection: HubConnection;

  constructor(
    private signalRConnectionService: SignalRConnectionService,
    private httpClient:HttpClient) {

    signalRConnectionService.isConnected$.subscribe(res => {
      if (res) {

        this.hubConnection = signalRConnectionService.connection;

        signalRConnectionService.connection.on("BroadcastMessage", res=> {
          if (res) {
            res = JSON.parse(res);
            this.message$.emit(res);
          }
        });

        signalRConnectionService.connection.on("InviteToPrivateChat", res=> {
          if (res) {
            this.inviterToChat$.emit(res);
           // res = JSON.parse(res);
           // this.message$.emit(res);
          }
        });
      }
    });

  }

  sendMessage(message):Promise<any>{
    if (this.hubConnection) {
      return this.hubConnection.invoke("SendMessage",message);
    }
    else return Promise.reject('connection is null');
  }

  getNumberOfMessages(number:number):Observable<Array<SendMessage>>{
    let url ="/api/chat/getPublicMessages?numberOfMessages="+number;
    return this.httpClient.get<Array<SendMessage>>(url);
  }

  convertToChatMsg(message: SendMessage): ChatMessage {

    let user = localStorage.getItem("userName");
    let msg: ChatMessage =new ChatMessage();
    msg.user.name = message.userName ? message.userName : "Nan";
    msg.date = message.date ? new Date(message.date) : new Date(Date.now());
    msg.text = message.content ? message.content : "Nan";
    msg.reply = (message.userName !== user) ? true : false;


    return msg;

  }

}
