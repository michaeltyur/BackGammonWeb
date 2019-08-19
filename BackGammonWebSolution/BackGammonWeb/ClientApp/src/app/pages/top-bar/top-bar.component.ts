import { Component, OnInit, OnDestroy } from '@angular/core';
import { AuthService } from 'src/app/shared/services/auth.service';
import { Subscription } from 'rxjs';
import { Router } from '@angular/router';
import { NbToastrService } from '@nebular/theme';
import { SignalRConnectionService } from 'src/app/shared/services/signal-r-connection.service';

@Component({
  selector: 'app-top-bar',
  templateUrl: './top-bar.component.html',
  styleUrls: ['./top-bar.component.scss']
})
export class TopBarComponent implements OnInit, OnDestroy {

  subscription = new Subscription();
  isConnected: boolean = false;
  isAuthenticated: boolean = false;
  userName: string;

  constructor(
    private authService: AuthService,
    private router: Router,
    private nbToastrService: NbToastrService,
    private signalRConnectionService: SignalRConnectionService) { }

  ngOnInit() {

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

  ngOnDestroy() {
    this.subscription.unsubscribe();
  }

  logout(): void {
    this.authService.logout();
    this.nbToastrService.warning('', 'You are logged out');

  }

}
