import {NgModule} from '@angular/core';
import {RouterModule, Routes} from '@angular/router';
import {DashboardComponent} from "./dashboard/dashboard.component";
import {AnalyticsComponent} from "./analytics/analytics.component";
import {LoginComponent} from "./login/login.component";
import {authGuard} from "./gurads/auth.guard";

const routes: Routes = [
    {path: '', redirectTo: 'dashboard', pathMatch: 'full'},
    {
        path: 'dashboard',
        component: DashboardComponent,
        canActivate: [authGuard]
    },
    {path: 'analytics', component: AnalyticsComponent},
    {path: 'login', component: LoginComponent},
    {path: '**', redirectTo: 'dashboard'}
];

@NgModule({
    imports: [RouterModule.forRoot(routes)],
    exports: [RouterModule]
})
export class AppRoutingModule {
}
