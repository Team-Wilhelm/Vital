import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import {NgOptimizedImage} from "@angular/common";
import { AnalyticsComponent } from './analytics/analytics.component';
import {CardComponent} from "./card/card.component";
import {CurrentCycleComponent} from "./dashboard/current-cycle/current-cycle.component";
import {MedicationListItemComponent} from "./dashboard/medication-list-item/medication-list-item.component";
import { HttpClientModule} from "@angular/common/http";

@NgModule({
  declarations: [
    AppComponent,
    DashboardComponent,
    AnalyticsComponent,
    CardComponent,
    CurrentCycleComponent,
    MedicationListItemComponent
  ],
    imports: [
        BrowserModule,
        AppRoutingModule,
        NgOptimizedImage,
      HttpClientModule
    ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
