import { Injectable, EventEmitter } from '@angular/core';
import { SignalRConnectionService } from './signal-r-connection.service';
import { HubConnection } from '@aspnet/signalr';
import { SendMessage } from '../models/message-send';

@Injectable({
  providedIn: 'root'
})
export class ChatService {

  message$ = new EventEmitter();

  private hubConnection: HubConnection;

  constructor(private signalRConnectionService: SignalRConnectionService) {

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

}
