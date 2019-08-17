import { Injectable } from '@angular/core';
import { SignalRConnectionService } from './signal-r-connection.service';
import { EventEmitter } from 'events';

@Injectable({
  providedIn: 'root'
})
export class ChatService {

  mwssage$ = new EventEmitter();
  constructor(private signalRConnectionService: SignalRConnectionService) {

    signalRConnectionService.isConnected$.subscribe(res => {
      if (res) {
        signalRConnectionService.connection.on("BroadcastMessage", (res) => {
          if (res) {
            this.mwssage$.emit(res);
          }
        })
      }
    })
  }

}
