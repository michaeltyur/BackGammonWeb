import { Component, OnInit, OnDestroy, Input } from '@angular/core';
import { ChatMessage } from 'src/app/shared/models/chat-message';
import { Subscription } from 'rxjs';
import { UserService } from 'src/app/shared/services/user.service';
import { User } from 'src/app/shared/models/user';
import { ChatService } from 'src/app/shared/services/chat.service';
import { SendMessage } from 'src/app/shared/models/message-send';

@Component({
  selector: 'app-chat',
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.scss']
})
export class ChatComponent implements OnInit, OnDestroy {

  subscription = new Subscription();
  messages = new Array<ChatMessage>();

  @Input() isPrivate;
  constructor(
    private userService: UserService,
    private chatService: ChatService) { }

  ngOnInit() {
    this.chatService.message$.subscribe(res => {
      if (res) {

        let msg = <SendMessage>res;
        // let msg = <ChatMessage>res;
        this.convertToChatMsg(msg);
      }
    });
  }
  ngOnDestroy() {
    this.subscription.unsubscribe();
  }


  sendMessage(event): void {
    this.chatService.sendMessage(event.message).catch(err => console.error(err));
  }

  convertToChatMsg(message: SendMessage): void {

    let user = localStorage.getItem("userName");
    let msg: ChatMessage =new ChatMessage();
    msg.user.name = message.userName ? message.userName : "Nan";
    msg.date = message.date ? new Date(message.date) : new Date(Date.now());
    msg.message = message.content ? message.content : "Nan";
    msg.reply = (msg.sender !== user) ? true : false;


    this.messages.push(msg);

  }

}
