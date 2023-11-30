import { Component } from '@angular/core';
import {Router} from "@angular/router";
import {TokenService} from "./services/token.service";

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
})
export class AppComponent {
  title = 'Vital';

  constructor(public tokenService: TokenService, private router: Router) {
  }

  async profile() {
    await this.router.navigate(['/profile']);
  }

  async logout() {
    this.tokenService.logout();
    await this.router.navigate(['/login']);
  }
}
