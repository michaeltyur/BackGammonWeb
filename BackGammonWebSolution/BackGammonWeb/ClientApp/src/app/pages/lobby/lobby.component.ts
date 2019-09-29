import { Component, OnInit, OnDestroy, Inject } from '@angular/core';
import { User } from 'src/app/shared/models/user';
import { Subscription } from 'rxjs';
import { UserService } from 'src/app/shared/services/user.service';
import { SignalRConnectionService } from 'src/app/shared/services/signal-r-connection.service';
import { ISendMessage } from 'src/app/shared/models/message-send';
import { ChatService } from 'src/app/shared/services/chat.service';
import { IDictionary, Dictionary } from 'src/app/shared/models/dictionary';
import { ChatMessage } from 'src/app/shared/models/chat-message';
import { DOCUMENT } from '@angular/common';
import { ChatInvitation } from 'src/app/shared/models/chat-invitation';
import { NbToastrService } from '@nebular/theme';

@Component({
  selector: 'app-lobby',
  templateUrl: './lobby.component.html',
  styleUrls: ['./lobby.component.scss']
})
export class LobbyComponent implements OnInit, OnDestroy {

  subscription = new Subscription();
  usersOnLine: Array<User>=[];
  usersOffLine: Array<User>=[];
  currentChat: Array<ChatMessage> = [];
  allChatDictionary: IDictionary;
  chatTitle: string = "Public Chat";
  groupName: string = "public";
  isChat: boolean = true;
  isMobile: boolean = false;

  constructor(
    private nbToastrService: NbToastrService,
    private userService: UserService,
    private chatService: ChatService,
    private signalRConnectionService: SignalRConnectionService
  ) {
    this.signalRConnectionService.startConnection();
  }

  ngOnInit() {

    if (window.innerWidth < 600) {
      this.isMobile = true;
    }

    //Users
    this.subscription.add(this.userService.users$.subscribe(res => {
      if (res) {
        this.setUsersArrays(res);
      }
    }));

    this.subscription.add(this.chatService.users$.subscribe(res => {
      if (res) {
        this.setUsersArrays(res);
      }
    }));

    this.subscription.add(this.chatService.invitationToChat$.subscribe(res => {
      if (res) {
        this.openPrivateChatFromRemote(res.userName, res.groupName);
      }
    }, error => console.error(error)));

    this.subscription.add(this.chatService.privateChatClosedByOtherUser$.subscribe(res => {
      if (res) {
        this.closePrivateChatByRemote(res.userName, res.groupName);
      }
    }, err => console.error(err)
    ));

    this.getAllUser();

    this.signalRConnectionService.connection.on("BroadcastMessage", res => {
      if (res) {
        res = JSON.parse(res);
        let sendMessage = <ISendMessage>res;
        let msg = this.chatService.convertToChatMsg(sendMessage);
        let chat = this.allChatDictionary.getByKey(sendMessage.groupName)
        chat.push(msg);
      }
    });

    this.subscription.add(this.chatService.switchToChat$.subscribe(res => {
      if (res) {
        this.swichToChat(res.userID, res.groupName);
      }
    }));

    this.chatService.invitationToChat$.subscribe(res => {
      this.addChatToArray(res.groupName);
    }, error => console.error(error))

    this.subscription.add(this.chatService.closePrivateChat$.subscribe(res => {
      if (res) {
        this.closePrivateChat(res);
      }
    }, error => {
      console.error(error)
    }));

    this.subscription.add(this.chatService.privateChatClosedByOtherUser$.subscribe(res => {
      if (res) {
        this.closePrivateChat(res.groupName);
      }
    }, error => {
      console.error(error)
    }));


    this.allChatDictionary = new Dictionary<ChatMessage>();

    this.getNumberOfMessages("public", 50);

  }

  ngOnDestroy() {
    this.subscription.unsubscribe();
  }

  getNumberOfMessages(key: string, numberOfMsgs: number): void {

    this.subscription
      .add(this.chatService.getNumberOfMessages(numberOfMsgs)
        .subscribe((res: Array<ISendMessage>) => {
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

  swichToChat(userID: number, groupName: string): void {
    if (groupName) {
      if (groupName === 'public') {
        this.groupName = "public";
        this.chatTitle = "Public Chat";
        this.nbToastrService.default('', 'switch to public chat');
      }
      else {
        this.groupName = groupName;
        this.chatTitle = "Chat with " + this.usersOnLine.find(u=>u.userID=userID).userName;
      }

      let chat = <ChatMessage[]>this.allChatDictionary.getByKey(groupName);
      if (chat) {
        this.currentChat = chat;
      }
      else {
        this.addChatToArray(groupName);
      }

      if (this.isMobile) this.isChat = true;
    }
  }

  addChatToArray(key: string): void {
    if (key && !this.allChatDictionary.containsKey(key)) {
      this.allChatDictionary.add(key, new Array<ChatMessage>());
    }


  }

  closePrivateChat(groupName: string): void {
    if (this.allChatDictionary.containsKey(groupName)) {
      this.allChatDictionary.remove(groupName);
    }
  }

  getAllUser(): void {

    this.subscription.add(this.userService.getAllUser().subscribe(res => {
      if (res) {
        this.setUsersArrays(res);
      }
    }, error => {
      console.error(error);
    }));

  }

  setUsersArrays(allUsers: Array<User>): void {
    if (allUsers && allUsers.length) {

      this.usersOnLine = [];
      this.usersOffLine = [];

      allUsers.forEach(element => {
        if (element.isOnline) {
          this.usersOnLine.push(element);
        }
        else {
          this.usersOffLine.push(element);
        }
      });
    }

  }

  openPrivateChatFromRemote(userName: string, groupName: string): void {
    if (userName) {
      let user = this.usersOnLine.find(el => el.userName === userName)
      if (user) {
        user.haveNewPrivateChat = true;
        user.groupName = groupName;
      }
    }
    else console.error("user name is null");
  }
  closePrivateChatByRemote(userName: string, groupName: string): void {
    if (!userName) {
      console.error("userName is null");
      return;
    }
    if (!groupName) {
      console.error("groupName is null");
      return;
    }
    let user = this.usersOnLine.find(user => user.userName === userName);
    if (user) {
      user.haveNewPrivateChat = false;
    }
  }



}
