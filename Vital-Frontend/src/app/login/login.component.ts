import {Component} from "@angular/core";
import {TokenService} from "../services/token.service";
import {Router} from "@angular/router";

@Component({
  selector: 'app-login',
  template: `
    <div class="flex justify-center items-center">
      <div class="flex flex-col min-h-[100px] justify-around" aria-hidden="true">
        <input type="email" class="bg-card">
        <input type="password" class="bg-card">
        <button class="bg-primary text-white" (click)="login()">Login</button>
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
