import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { NbDialogService } from '@nebular/theme';
import { UsersComponent } from 'src/app/elements/users/users.component';
import { UsersModalComponent } from 'src/app/elements/modals/users-modal/users-modal.component';

@Component({
  selector: 'app-game-lobby',
  templateUrl: './game-lobby.component.html',
  styleUrls: ['./game-lobby.component.scss']
})
export class GameLobbyComponent implements OnInit, OnDestroy {
  subscription = new Subscription();
  isMobile: boolean = false;
  constructor(private nbDialogService:NbDialogService) { }

  ngOnInit() {
    if (window.innerWidth < 600) {
      this.isMobile = true;
    }
  }
  ngOnDestroy() {
    this.subscription.unsubscribe();
  }

  openUserWindow():void{
    const dialogRef = this.nbDialogService.open(UsersModalComponent);
  }

}
