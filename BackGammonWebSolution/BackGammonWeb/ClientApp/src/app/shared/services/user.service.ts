import { Injectable, EventEmitter } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, pipe } from 'rxjs';
import { IUser } from '../models/user';
import { SignalRConnectionService } from './signal-r-connection.service';
import { HubConnection } from '@aspnet/signalr';
import { ChatInvitation } from '../models/chat-invitation';
import { tap } from 'rxjs/operators';
import { IPrivateChatByUser } from '../models/private-chat-by-user';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  users$ = new EventEmitter();
  users: Array<IUser> = [];
  privateChatByUser$ = new EventEmitter<Array<IPrivateChatByUser>>();

  constructor(
    private httpClient: HttpClient,
    private signalRConnectionService: SignalRConnectionService
  ) {

    signalRConnectionService.isConnected$.subscribe(res => {
      if (res) {

        signalRConnectionService.connection.on("UpdateUsers", (res: Array<IUser>) => {
          if (res) {
            //res = JSON.parse(res);
            this.users$.emit(res);
          }
        });

        signalRConnectionService.connection.on("UpdatePrivateChatsByUser", (res: Array<IPrivateChatByUser>) => {
          if (res) {
            //res = JSON.parse(res);
            this.privateChatByUser$.emit(res);
          }
        });
      }
    });

  }

  getAllUser(): Observable<any> {
    let url = "/api/user/getAllUsers";
    return this.httpClient.get(url).pipe(tap(res => {

      if (res && res.length) {
        this.users = res;
      }
    }));
  }
}
