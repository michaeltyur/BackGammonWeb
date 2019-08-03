import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { User } from '../models/user';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  public user: User;

  constructor(private httpClient: HttpClient) { }

  login(form): Observable<any> {
    let url = "/api/Authentication/Login";
    return this.httpClient.post(url, form).pipe(map(res => {
      if (res) {
        this.user=<User>res;
        localStorage.setItem('token', this.user.token);
      }
    }));
  }

  public getToken(): string {
    return localStorage.getItem('token');
  }


}
