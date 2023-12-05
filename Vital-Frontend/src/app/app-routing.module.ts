import {NgModule} from '@angular/core';
import {RouterModule, Routes} from '@angular/router';
import {DashboardComponent} from "./dashboard/dashboard.component";
import {AnalyticsComponent} from "./analytics/analytics.component";
import {authGuard} from "./gurads/auth.guard";
import {AddMetricPageComponent} from "./add-metric-page/add-metric-page.component";
import {AuthComponent} from "./auth/auth.component";
import {ProfileComponent} from "./profile/profile.component";
import {FirstLoginComponent} from "./first-login-page/first-login.component";
import {initialLoginGuard} from "./gurads/initial-login.guard";

const routes: Routes = [
  {path: '', redirectTo: 'dashboard', pathMatch: 'full'},
  {
    path: 'dashboard',
    component: DashboardComponent,
    canActivate: [authGuard]
  },
  {
    path: 'analytics',
    component: AnalyticsComponent,
    canActivate: [authGuard]
  },
  {
    path: 'add-metric',
    component: AddMetricPageComponent,
    pathMatch: 'full',
    canActivate: [authGuard]
  },
  {
    path: 'initial-login',
    component: FirstLoginComponent,
    pathMatch: 'full',
    canActivate: [initialLoginGuard]
  },
  {
    path: 'profile',
    component: ProfileComponent,
    canActivate: [authGuard]
  },
  {path: 'login', component: AuthComponent},
  {path: '**', redirectTo: 'dashboard'}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule {
}
