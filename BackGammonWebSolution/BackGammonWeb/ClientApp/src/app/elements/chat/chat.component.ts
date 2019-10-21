import { Component, OnInit, OnDestroy, Input } from '@angular/core';
import { ChatMessage } from 'src/app/shared/models/chat-message';
import { Subscription } from 'rxjs';
import { UserService } from 'src/app/shared/services/user.service';
import { ChatService } from 'src/app/shared/services/chat.service';
import { ISendMessage, SendMessage } from 'src/app/shared/models/message-send';
import { SignalRConnectionService } from 'src/app/shared/services/signal-r-connection.service';
import { HubConnection } from '@aspnet/signalr';

@Component({
  selector: 'app-chat',
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.scss']
})
export class ChatComponent implements OnInit, OnDestroy {

  subscription = new Subscription();
  @Input() messages: Array<ChatMessage> = [];
  @Input() groupName: string;

  constructor(
    private userService: UserService,
    private chatService: ChatService,
    private signalRConnectionService: SignalRConnectionService) { }

  ngOnInit() {

    // this.signalRConnectionService.connection.on("BroadcastMessage", res => {
    //   if (res) {
    //     res = JSON.parse(res);
    //     let msg = this.chatService.convertToChatMsg(<ISendMessage>res);
    //     this.messages.push(msg);
    //   }
    // });


  }
  ngOnDestroy() {
    this.subscription.unsubscribe();
  }

  sendMessage(event): void {
    if (event.message) {
      let msg = new SendMessage();
      msg.content = event.message;
      msg.groupName = this.groupName;
      msg.userName=localStorage.getItem('userName');
      this.chatService.sendMessage(msg).then().catch(err => console.error(err));
    }
  }

}
