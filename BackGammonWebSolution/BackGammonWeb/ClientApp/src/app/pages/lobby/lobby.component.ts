import { Component, OnInit, OnDestroy, Inject } from '@angular/core';
import { User } from 'src/app/shared/models/user';
import { Subscription } from 'rxjs';
import { UserService } from 'src/app/shared/services/user.service';
import { SignalRConnectionService } from 'src/app/shared/services/signal-r-connection.service';
import { SendMessage } from 'src/app/shared/models/message-send';
import { ChatService } from 'src/app/shared/services/chat.service';
import { IDictionary, Dictionary } from 'src/app/shared/models/dictionary';
import { ChatMessage } from 'src/app/shared/models/chat-message';
import { DOCUMENT } from '@angular/common';

@Component({
  selector: 'app-lobby',
  templateUrl: './lobby.component.html',
  styleUrls: ['./lobby.component.scss']
})
export class LobbyComponent implements OnInit, OnDestroy {
  elem;
  subscription = new Subscription();
  currentChat: Array<ChatMessage> = [];
  allChatDictionary: IDictionary;
  chatTitle: string = "Public Chat";
  isChat: boolean = true;

  constructor(
    private userService: UserService,
    private signalRConnectionService: SignalRConnectionService,
    private chatService: ChatService,
    @Inject(DOCUMENT) private document: any
  ) {
    this.signalRConnectionService.startConnection();
  }

  ngOnInit() {
    // Full Screen Browser
    this.elem = document.documentElement;
    if (window.innerWidth < 800) {
      this.openFullscreen();
    }

    this.allChatDictionary = new Dictionary<ChatMessage>();

    this.getNumberOfMessages("public", 50);

  }

  ngOnDestroy() {
    this.subscription.unsubscribe();
  }

  getNumberOfMessages(key: string, numberOfMsgs: number): void {

    this.subscription
      .add(this.chatService.getNumberOfMessages(numberOfMsgs)
        .subscribe((res: Array<SendMessage>) => {
          if (res && res.length) {

            let chatArray = new Array<ChatMessage>();

            res.forEach(element => {
              chatArray.push(this.chatService.convertToChatMsg(element));
            });

            this.currentChat = chatArray;

            this.allChatDictionary.add(key, chatArray);
          }
        }, err => console.error(err)));

  }

  openPrivateChat(user: User): void {
    if (user && this.signalRConnectionService.connection) {
      this.signalRConnectionService.connection.invoke('AddToGroup', user.userName).then(res => {
        let chatArray = new Array<ChatMessage>();
      }).catch(err => console.error(err));
    }
  }

  openFullscreen() {
    if (this.elem.requestFullscreen) {
      this.elem.requestFullscreen();
    } else if (this.elem.mozRequestFullScreen) {
      /* Firefox */
      this.elem.mozRequestFullScreen();
    } else if (this.elem.webkitRequestFullscreen) {
      /* Chrome, Safari and Opera */
      this.elem.webkitRequestFullscreen();
    } else if (this.elem.msRequestFullscreen) {
      /* IE/Edge */
      this.elem.msRequestFullscreen();
    }
  }

  /* Close fullscreen */
  closeFullscreen() {
    if (this.document.exitFullscreen) {
      this.document.exitFullscreen();
    } else if (this.document.mozCancelFullScreen) {
      /* Firefox */
      this.document.mozCancelFullScreen();
    } else if (this.document.webkitExitFullscreen) {
      /* Chrome, Safari and Opera */
      this.document.webkitExitFullscreen();
    } else if (this.document.msExitFullscreen) {
      /* IE/Edge */
      this.document.msExitFullscreen();
    }
  }

}
