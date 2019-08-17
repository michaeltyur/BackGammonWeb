import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Subscription } from 'rxjs';
import { AuthService } from 'src/app/shared/services/auth.service';
import { NbToastrService } from '@nebular/theme';
import { Router } from '@angular/router';
import { onlyDigitsValidator } from 'src/app/shared/validators/only-digits.validator';

@Component({
  selector: 'app-registration',
  templateUrl: './registration.component.html',
  styleUrls: ['./registration.component.scss']
})
export class RegistrationComponent implements OnInit {

  subscribtion = new Subscription()
  userForm: FormGroup;
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
    this.userForm = this.fb.group({
      firstName: ['', [Validators.required, Validators.minLength(2)]],
      lastName: ['', [Validators.required, Validators.minLength(2)]],
      userName: ['', [Validators.required, Validators.minLength(3)]],
      password: ['', [Validators.required, Validators.minLength(3), onlyDigitsValidator()]],
    });
  }

  registration(): void {
    if (this.userForm.valid) {
      this.loginLoading = true;
      this.authService.registration(this.userForm.value).subscribe(res=>{
        if (res['token']) {
          this.nbToastrService.success('', 'You are registred successfully');
          this.loginLoading = false;
          this.router.navigate(['lobby']);
        }
        else if (res['error']) {
          this.nbToastrService.danger('', res['error']);
        }
      },error=>{
        console.error(error);
          this.loginLoading = false;
      })
    }
    else {
      this.nbToastrService.warning('', 'One or more parameters are incorrect');
    }
  }

  get firstName() {
    return this.userForm.get('firstName');
  }
  get lastName() {
    return this.userForm.get('lastName');
  }
  get userName() {
    return this.userForm.get('userName');
  }
  get password() {
    return this.userForm.get('password');
  }
}
