import {NgModule} from '@angular/core';
import {BrowserModule} from '@angular/platform-browser';

import {AppRoutingModule} from './app-routing.module';
import {AppComponent} from './app.component';
import {DashboardComponent} from './dashboard/dashboard.component';
import {FullCalendarModule} from '@fullcalendar/angular';

import {DatePipe, NgOptimizedImage} from "@angular/common";
import {AnalyticsComponent} from './analytics/analytics.component';
import {CardComponent} from "./card/card.component";
import {CurrentCycleComponent} from "./dashboard/current-cycle/current-cycle.component";
import {MedicationListItemComponent} from "./dashboard/medication-list-item/medication-list-item.component";
import {HTTP_INTERCEPTORS, HttpClientModule} from "@angular/common/http";
import {AuthHttpInterceptor} from "./interceptors/auth-http-interceptor";
import {TokenService} from "./services/token.service";
import {CycleService} from "./services/cycle.service";
import {JWT_OPTIONS, JwtHelperService} from "@auth0/angular-jwt";
import {FormsModule, ReactiveFormsModule} from "@angular/forms";
import {AuthComponent} from "./auth/auth.component";
import {CalendarComponent} from './calendar/calendar.component';
import {AddMetricPageComponent} from './add-metric-page/add-metric-page.component';
import {MetricListItemComponent} from "./dashboard/metric-list-item/metric-list-item.component";
import {MetricSelectionItemComponent} from "./add-metric-page/metric-selection-item/metric-selection-item.component";
import {BrowserAnimationsModule} from "@angular/platform-browser/animations";
import {RegisterCardComponent} from "./auth/register/registerCard.component";
import {LoginCardComponent} from "./auth/login/loginCard.component";
import {ToastComponent} from "./toasts/toast.component";
import {ProfileComponent} from "./profile/profile.component";
import {ApplicationStatCardComponent} from "./stat-card/application-stat-card.component";
import {StatCardComponent} from "./card/stat-card.component";
import {FirstLoginComponent} from "./first-login-page/first-login.component";
import {PasswordInputComponent} from "./auth/password-input.component";
import AccountService from "./services/account.service";
import {ForgotPasswordComponent} from "./auth/forgot-password/forgot-password.component";
import {ResetPasswordComponent} from "./auth/reset-password/reset-password.component";
import {VerifyEmailComponent} from "./auth/verify-email/verify-email.component";
import {ActivatedRoute, ActivatedRouteSnapshot} from "@angular/router";

@NgModule({
  declarations: [
    AppComponent,
    DashboardComponent,
    AnalyticsComponent,
    CardComponent,
    StatCardComponent,
    CurrentCycleComponent,
    MedicationListItemComponent,
    CalendarComponent,
    AddMetricPageComponent,
    AuthComponent,
    RegisterCardComponent,
    LoginCardComponent,
    MetricListItemComponent,
    MetricSelectionItemComponent,
    ToastComponent,
    MetricSelectionItemComponent,
    ProfileComponent,
    ApplicationStatCardComponent,
    FirstLoginComponent,
    PasswordInputComponent,
    ApplicationStatCardComponent,
    ForgotPasswordComponent,
    ResetPasswordComponent,
    VerifyEmailComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    NgOptimizedImage,
    HttpClientModule,
    ReactiveFormsModule,
    FullCalendarModule,
    FormsModule,
    NgOptimizedImage,
    BrowserAnimationsModule
  ],
  providers: [
    TokenService,
    AccountService,
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthHttpInterceptor,
      multi: true
    },
    {provide: JWT_OPTIONS, useValue: JWT_OPTIONS},
    JwtHelperService,
    DatePipe,
  ],
  bootstrap: [AppComponent]
})

export class AppModule {
}
