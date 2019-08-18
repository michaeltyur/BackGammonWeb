import { Injectable, EventEmitter } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map, tap } from 'rxjs/operators';
import { User } from '../models/user';
import { Router } from '@angular/router';
import { SignalRConnectionService } from './signal-r-connection.service';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  public user: User;

  isAuthenticated$ = new EventEmitter<boolean>();

  constructor(
    private httpClient: HttpClient,
    private router: Router,
    private signalRConnectionService: SignalRConnectionService) { }

  login(form): Observable<any> {
    let url = "/api/authentication/login";
    return this.httpClient.post(url, form).pipe(tap(res => {
      if (res['token']) {
        this.user = <User>res;
        localStorage.setItem('token', this.user.token);
        localStorage.setItem('userName', this.user.userName);
        this.isAuthenticated$.emit(true);
        this.signalRConnectionService.startConnection();
      }
    }));
  }

  logout(): void {
    let userName = localStorage.getItem('userName');
    if (userName) {
      let url = "/api/authentication/logout?userName=" + userName;
      this.httpClient.get(url).subscribe(res => {
        if (res['success']) {
        }
      }, error => {
        console.log(error);
      })
      localStorage.clear();
      this.signalRConnectionService.closeConnection();
      this.user = null;
      this.isAuthenticated$.emit(false);
      this.router.navigate(['login']);
    }

  }

  registration(user: User): Observable<any> {

    let url = "/api/authentication/registration";

    return this.httpClient.post(url, user).pipe(tap(res => {
      if (res['token']) {
        this.user = <User>res;
        localStorage.setItem('token', this.user.token);
        this.isAuthenticated$.emit(true);
      }
    }));

  }

  public getToken(): string {
      return localStorage.getItem('token');
  }
  isAuthenticated(): boolean {
    return localStorage.getItem('token') ? true : false;
  }


}
