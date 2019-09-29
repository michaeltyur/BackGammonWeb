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
  //serverUrl = "http://michaelt-001-site3.btempurl.com/backgammon";
  token: string;

  constructor() {
    this.isConnected$ = new EventEmitter<boolean>();
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
    }).catch( (err)=> {
      this.isConnected$.emit(false);
      // this.serverUrl = "http://michaelt-001-site3.btempurl.com/backgammon";
       console.error(err.toString());
       this.startConnection();
    });

    this.connection.onclose(err => {

      if (localStorage.getItem('token')) {
        this.startConnection();
      }
      this.isConnected$.emit(false);
    });

  }

  closeConnection(): void {
    if (this.connection) {
      this.connection.stop().then(() => {
        console.log("SignalR disconnected");
        this.isConnected$.emit(false);
      }).catch(err=>console.error(err))
    }


  }
}
