import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import {DashboardComponent} from "./dashboard/dashboard.component";
import {AddMetricPageComponent} from "./add-metric-page/add-metric-page.component";

const routes: Routes = [
  { path: '', component: DashboardComponent, pathMatch: 'full' },
  //TODO check routing
  { path: 'add-metric', component: AddMetricPageComponent, pathMatch: 'full' },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
