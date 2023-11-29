import {NgModule} from '@angular/core';
import {RouterModule, Routes} from '@angular/router';
import {DashboardComponent} from "./dashboard/dashboard.component";
import {AnalyticsComponent} from "./analytics/analytics.component";
import {authGuard} from "./gurads/auth.guard";
import {AddMetricPageComponent} from "./add-metric-page/add-metric-page.component";
import {AuthComponent} from "./auth/auth.component";

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
      pathMatch: 'full'
    },
    {path: 'login', component: AuthComponent},
    {path: '**', redirectTo: 'dashboard'},
];

@NgModule({
    imports: [RouterModule.forRoot(routes)],
    exports: [RouterModule]
})
export class AppRoutingModule {
}
