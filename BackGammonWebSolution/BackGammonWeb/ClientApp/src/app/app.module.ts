import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { AppComponent } from './app.component';
import { LoginComponent } from './pages/login/login.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NbThemeModule, NbLayoutModule, NbCardModule, NbButtonModule, NbInputModule, NbPopoverModule, NbToastrModule, NbSpinnerModule, NbChatModule, NbWindowModule } from '@nebular/theme';
import { NbEvaIconsModule } from '@nebular/eva-icons';
import { ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { JwtInterceptor } from './shared/helpers/auth-interceptor';
import { BackgammonGameComponent } from './pages/backgammon-game/backgammon-game.component';
import { RegistrationComponent } from './pages/registration/registration.component';
import { AuthGuard } from './shared/guardes/auth-guard.service';
import { TopBarComponent } from './pages/top-bar/top-bar.component';
import { ChatComponent } from './pages/chat/chat.component';
import { LobbyComponent } from './pages/lobby/lobby.component';

const routes: Routes = [

  { path: '',
    redirectTo: '/login',
    pathMatch: 'full'
  },
  {
    path: 'login',
    component: LoginComponent
  },
  {
    path: 'registration',
    component: RegistrationComponent
  },
  {
    path: 'lobby',
    component: LobbyComponent,
    canActivate: [AuthGuard]
  },
  {
    path: 'game',
    component: BackgammonGameComponent,
    canActivate: [AuthGuard]
  },
  { path: '**', component: LoginComponent }
];

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    BackgammonGameComponent,
    RegistrationComponent,
    TopBarComponent,
    ChatComponent,
    LobbyComponent
  ],
  imports: [
    BrowserModule,
    RouterModule.forRoot(routes),
    BrowserAnimationsModule,
    NbThemeModule.forRoot({ name: 'cosmic' }),
    NbLayoutModule,
    NbEvaIconsModule,
    NbCardModule,
    NbButtonModule,
    NbInputModule,
    NbPopoverModule,
    FormsModule ,
    ReactiveFormsModule,
    HttpClientModule,
    NbToastrModule.forRoot(),
    NbSpinnerModule,
    NbChatModule,
    NbWindowModule.forRoot()
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true },
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
