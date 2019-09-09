import { Injectable, EventEmitter } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { User } from '../models/user';
import { SignalRConnectionService } from './signal-r-connection.service';
import { HubConnection } from '@aspnet/signalr';
import { ChatInvitation } from '../models/chat-invitation';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  users$ = new EventEmitter();

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
      }
    });

  }

  getAllUser(): Observable<any> {
    let url = "/api/user/getAllUsers";
    return this.httpClient.get(url);
  }
}
