import { Injectable, EventEmitter } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from "@aspnet/signalr";

@Injectable({
  providedIn: 'root'
})
export class SignalRConnectionService {

  connection: HubConnection;
  isConnected: boolean = false;
  isConnected$ = new EventEmitter<boolean>();
  serverUrl = "http://localhost:50740/backgammon";
  token: string;

  constructor() {

  }

  startConnection(): void {


    this.connection = new HubConnectionBuilder()
      .withUrl(this.serverUrl,
        {
          accessTokenFactory: () => localStorage.getItem('token')
        })
      .build();

    this.connection.start().then(() => {
      console.log('SignalR Connected!');
      this.isConnected = true;
      this.isConnected$.emit(true);
    }).catch(function (err) {
      this.isConnected$.emit(false);
      return console.error(err.toString());
    });

    this.connection.onclose(err => {
      this.startConnection();
      this.isConnected$.emit(false);
    });

  }

  closeConnection(): void {
    if (this.connection) {
      this.connection.stop().then(() => {
        this.connection = null;
        this.isConnected$.emit(false);
      })
      this.isConnected$.emit(false);
    }


  }
}
