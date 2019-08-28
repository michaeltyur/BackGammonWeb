import { Injectable, EventEmitter } from '@angular/core';
import { SignalRConnectionService } from './signal-r-connection.service';
import { HubConnection } from '@aspnet/signalr';
import { SendMessage } from '../models/message-send';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class ChatService {

  message$ = new EventEmitter();

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
        })
      }
    })

  }

  sendMessage(message):Promise<any>{
    if (this.hubConnection) {
      return this.hubConnection.invoke("SendMessage",message);
    }
    else return Promise.reject('connection is null');
  }

  getNumberOfMessages(number:number):Observable<any>{
    let url ="/api/chat/getPublicMessages?numberOfMessages="+number;
    return this.httpClient.get(url);
  }

}
