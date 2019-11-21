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
  gameTableArray=[];
  constructor(private nbDialogService: NbDialogService) { }

  ngOnInit() {
    if (window.innerWidth < 600) {
      this.isMobile = true;
    }
    this.fillGameTable();
  }
  ngOnDestroy() {
    this.subscription.unsubscribe();
  }

  openUserWindow(): void {
    const dialogRef = this.nbDialogService.open(UsersModalComponent);
  }

  fillGameTable(): void {
    this.gameTableArray = [];

    for (let i = 0; i < 27; i++) {
      let element  = this.fillSingleArray();
      this.gameTableArray.push(element);
    }
  }
  fillSingleArray(): Array<boolean> {
    let array = [];
    for (let index = 0; index < 6; index++) {
      if (index%2===0) {
        array.push(true);
      }
      else{
        array.push(false);
      }
    }
    return array;
  }
}
