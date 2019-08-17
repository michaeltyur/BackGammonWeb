import { Component, OnInit, OnDestroy } from '@angular/core';
import { ChatMessage } from 'src/app/shared/models/chat-message';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-chat',
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.scss']
})
export class ChatComponent implements OnInit ,OnDestroy{
  messages = new Array<ChatMessage>();
  subscription = new Subscription();
  constructor() { }

  ngOnInit() {
  }
  ngOnDestroy(){
    this.subscription.unsubscribe();
  }

  sendMessage(event):void{

  }

}
