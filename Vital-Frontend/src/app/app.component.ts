import { Component } from '@angular/core';
import {TokenService} from "./services/token.service";
import {addWarning} from "@angular-devkit/build-angular/src/utils/webpack-diagnostics";
import {Router} from "@angular/router";

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
})
export class AppComponent {
  title = 'Vital';

  constructor(public tokenService: TokenService, private router: Router) {
  }

  async logout() {
    this.tokenService.logout();
    await this.router.navigate(['/login']);
  }
}
