import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { User } from '../models/user';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  constructor(private httpClient:HttpClient) { }


  getAllUser():Observable<any>{
    let url="/api/user/getAllUsers";
    return this.httpClient.get(url);
  }
}
