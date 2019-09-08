import { Injectable, EventEmitter } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { User } from '../models/user';
import { SignalRConnectionService } from './signal-r-connection.service';
import { HubConnection } from '@aspnet/signalr';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  users$ = new EventEmitter();
  inviterToChat$ = new EventEmitter();

  constructor(
    private httpClient: HttpClient,
    private signalRConnectionService: SignalRConnectionService) {

    signalRConnectionService.isConnected$.subscribe(res => {
      if (res) {

        signalRConnectionService.connection.on("UpdateUsers", res => {
          if (res) {
            //res = JSON.parse(res);
            this.users$.emit(res);
          }
        });

        signalRConnectionService.connection.on("InviteToPrivateChat", res => {
          if (res) {
            this.inviterToChat$.emit(res);
          }
        });
      }
    });

  }

  getAllUser(): Observable<any> {
    let url = "/api/user/getAllUsers";
    return this.httpClient.get(url);
  }
}
