import { Component, OnInit, OnDestroy } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { onlyDigitsValidator } from 'src/app/shared/validators/only-digits.validator';
import { AuthService } from 'src/app/shared/services/auth.service';
import { NbToastrService } from '@nebular/theme';
import { Subscription } from 'rxjs';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit, OnDestroy {

  subscribtion = new Subscription()
  loginForm: FormGroup;
  loginLoading: boolean = false
  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private nbToastrService: NbToastrService,
    private router: Router) { }


  ngOnInit() {
    this.initForm();
  }
  ngOnDestroy() {
    this.subscribtion.unsubscribe();
  }

  initForm(): void {
    this.loginForm = this.fb.group({
      userName: ['misha', [Validators.required, Validators.minLength(3)]],
      password: ['123', [Validators.required, Validators.minLength(3), onlyDigitsValidator()]],
    });
  }

  login(): void {
    if (this.loginForm.valid) {
      this.loginLoading = true;
      this.subscribtion.add(this.authService.login(this.loginForm.value).subscribe(res => {
        if (res['token']) {
          this.nbToastrService.success('', 'You are logged successfully');
          this.loginLoading = false;
          this.router.navigate(['lobby']);
        }
        else if (res['error']) {
          this.nbToastrService.danger('', res['error']);
        }
      }, error => {
        if (error['status']===504) {
          this.nbToastrService.danger('', 'The server not responding');
        }
        console.error(error);
        this.loginLoading = false;
      }));

    }
    else {
      this.nbToastrService.warning('', 'One or more parameters are incorrect');
    }

  }

  registration(): void {
    this.router.navigate(['registration']);
  }

  get userName() {
    return this.loginForm.get('userName');
  }
  get password() {
    return this.loginForm.get('password');
  }

}
