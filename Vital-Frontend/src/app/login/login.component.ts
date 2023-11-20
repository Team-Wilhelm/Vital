import {Component} from "@angular/core";
import {TokenService} from "../services/token.service";
import {Router} from "@angular/router";

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
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

  //TODO: Add link to register page
  //TODO: Nicer login page for mobile, add background image
}
