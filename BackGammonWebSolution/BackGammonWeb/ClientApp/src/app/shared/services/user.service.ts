import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { User } from '../models/user';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  constructor(private httpClient:HttpClient) { }


  getAllUser():Observable<Array<User>>{
    let url="/api/user/getAllUser";
    return this.httpClient.get<Array<User>>(url);
  }
}
