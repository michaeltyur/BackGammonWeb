import { Injectable, EventEmitter } from '@angular/core';
import * as signalR from '@aspnet/signalr';

@Injectable({
  providedIn: 'root'
})
export class SignalRConnectionService {

  connection;
  isConnected: boolean = false;
  isConnected$ = new EventEmitter<boolean>();
  serverUrl = "/backgammon";

  constructor() {
  }

  startConnection(): void {
    this.connection = new signalR.HubConnectionBuilder()
      .configureLogging(signalR.LogLevel.Information)
      .withUrl(this.serverUrl)
      .build();

    this.connection.start().then(() => {
      console.log('SignalR Connected!');
      this.isConnected = true;
      this.isConnected$.emit(true);
    }).catch(function (err) {
      this.isConnected$.emit(false);
      return console.error(err.toString());
    });

  }

  closeConnection(): void {
    this.connection = null;
    this.isConnected$.emit(false);
  }
}
