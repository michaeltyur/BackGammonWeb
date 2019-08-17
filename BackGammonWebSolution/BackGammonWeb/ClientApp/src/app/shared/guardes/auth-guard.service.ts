import { Injectable } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { Router } from '@angular/router';
import { NbToastrService } from '@nebular/theme';


@Injectable({
  providedIn: 'root'
})
export class AuthGuard {

  constructor(
    public auth: AuthService,
    public router: Router,
    private nbToastrService: NbToastrService) { }
  canActivate(): boolean {
    if (!this.auth.isAuthenticated()) {
      this.nbToastrService.danger('',"You don't have permission to access")
      this.router.navigate(['login']);
      return false;
    }
    return true;
  }
}
