import {NgModule} from '@angular/core';
import {BrowserModule} from '@angular/platform-browser';

import {AppRoutingModule} from './app-routing.module';
import {AppComponent} from './app.component';
import {DashboardComponent} from './dashboard/dashboard.component';
import {NgOptimizedImage} from "@angular/common";
import {AnalyticsComponent} from './analytics/analytics.component';
import {CardComponent} from "./card/card.component";
import {CurrentCycleComponent} from "./dashboard/current-cycle/current-cycle.component";
import {MedicationListItemComponent} from "./dashboard/medication-list-item/medication-list-item.component";
import { HttpClientModule} from "@angular/common/http";
import {HTTP_INTERCEPTORS, HttpClientModule} from "@angular/common/http";
import {LoginComponent} from "./login/login.component";
import {AuthHttpInterceptor} from "./interceptors/auth-http-interceptor";
import {TokenService} from "./services/token.service";
import {CycleService} from "./services/cycle.service";
import {JWT_OPTIONS, JwtHelperService} from "@auth0/angular-jwt";
import {ReactiveFormsModule} from "@angular/forms";
import {RegisterComponent} from "./register/register.component";

@NgModule({
  declarations: [
    AppComponent,
    DashboardComponent,
    AnalyticsComponent,
    CardComponent,
    CurrentCycleComponent,
    MedicationListItemComponent,
    LoginComponent,
    RegisterComponent
  ],
    imports: [
        BrowserModule,
        AppRoutingModule,
        NgOptimizedImage,
        HttpClientModule,
        ReactiveFormsModule,
    ],
  providers: [
    TokenService,
    CycleService,
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthHttpInterceptor,
      multi: true
    },
    {provide: JWT_OPTIONS, useValue: JWT_OPTIONS},
    JwtHelperService
  ],
  bootstrap: [AppComponent]
})

export class AppModule {
}
