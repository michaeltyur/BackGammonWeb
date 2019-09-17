import { Component, OnInit, OnDestroy, Inject, AfterViewInit } from '@angular/core';
import { AuthService } from 'src/app/shared/services/auth.service';
import { Subscription } from 'rxjs';
import { Router } from '@angular/router';
import { NbToastrService } from '@nebular/theme';
import { SignalRConnectionService } from 'src/app/shared/services/signal-r-connection.service';
import { DOCUMENT } from '@angular/common';
import * as $ from 'jquery';
import { GameService } from 'src/app/shared/services/game.service';
import { ChatService } from 'src/app/shared/services/chat.service';

@Component({
  selector: 'app-top-bar',
  templateUrl: './top-bar.component.html',
  styleUrls: ['./top-bar.component.scss']
})
export class TopBarComponent implements OnInit, OnDestroy, AfterViewInit {

  elem;
  isGameMode: boolean = false;
  fullScreen: boolean;
  subscription = new Subscription();
  isConnected: boolean = false;
  isAuthenticated: boolean = false;
  userName: string;

  constructor(
    private gameService: GameService,
    private authService: AuthService,
    private chatService: ChatService,
    private router: Router,
    private nbToastrService: NbToastrService,
    private signalRConnectionService: SignalRConnectionService,
    @Inject(DOCUMENT) private document: any) { }

  ngOnInit() {

    // Full Screen Browser
    this.elem = document.documentElement;

    this.subscription.add(this.authService.isAuthenticated$.subscribe(res => {
      this.isAuthenticated = res;
      if (res) {
        this.userName = localStorage.getItem('userName');
      }
      else {
        this.userName = null;
      }

    }));

    this.subscription.add(this.signalRConnectionService.isConnected$.subscribe(res => {

      this.isConnected = res;

    }));

    if (this.authService.isAuthenticated()) {
      this.isAuthenticated = true;
      this.userName = localStorage.getItem('userName');
    }

    if (this.signalRConnectionService.isConnected) {
      this.isConnected = true;
    }

  }

  ngAfterViewInit() {
    // if (window.innerWidth < 800) {
    //     document.getElementById("fullScreen-button").click();
    //    }

  }


  ngOnDestroy() {
    this.subscription.unsubscribe();
  }

  logout(): void {
    this.authService.logout();
    this.nbToastrService.warning('', 'You are logged out');

  }

  openFullscreen() {
    this.fullScreen = true;
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
    this.fullScreen = false;
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

  switchToGame(): void {
    this.router.navigate(['game-lobby']);
    this.isGameMode = true;
    this.gameService.gameModeOn$.emit(true);
  }

  switchToChat(): void {
    this.router.navigate(['lobby']);
    this.isGameMode = false;
    this.gameService.gameModeOn$.emit(false);
  }

}
