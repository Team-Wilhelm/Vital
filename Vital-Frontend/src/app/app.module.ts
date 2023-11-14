import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import {NgOptimizedImage} from "@angular/common";
import { AnalyticsComponent } from './analytics/analytics.component';
import {CardComponent} from "./card/card.component";

@NgModule({
  declarations: [
    AppComponent,
    DashboardComponent,
    AnalyticsComponent,
    CardComponent
  ],
    imports: [
        BrowserModule,
        AppRoutingModule,
        NgOptimizedImage
    ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
