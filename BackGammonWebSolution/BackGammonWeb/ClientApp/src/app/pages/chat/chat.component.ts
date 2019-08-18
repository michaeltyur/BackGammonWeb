import { Component, OnInit, OnDestroy, Input } from '@angular/core';
import { ChatMessage } from 'src/app/shared/models/chat-message';
import { Subscription } from 'rxjs';
import { UserService } from 'src/app/shared/services/user.service';
import { User } from 'src/app/shared/models/user';

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
    private userService: UserService) { }

  ngOnInit() {
  }
  ngOnDestroy() {
    this.subscription.unsubscribe();
  }


  sendMessage(event): void {

  }

}
