import {Component} from "@angular/core";
import {TokenService} from "../services/token.service";
import {Router} from "@angular/router";

@Component({
  selector: 'app-login',
  template: `
    <div class="relative">
      <div class="absolute top-0 right-0 bg-gray-100 p-2 rounded">
        <a routerLink="/register" class="text-blue-500">Register</a>
      </div>
    </div>
  `
})
export class LoginComponent {
  redirectUrl: string | null = null;

  constructor(private tokenService: TokenService, private router: Router) {
  }

  async login(): Promise<void> {
    await this.tokenService.login();
    if (this.tokenService.isAuthenticated()) {
      await this.router.navigate([this.redirectUrl || '']);
    }
  }
}
