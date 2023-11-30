import {Component, EventEmitter, Output} from "@angular/core";
import {TokenService} from "../../services/token.service";
import {Router} from "@angular/router";
import {AbstractControl, FormBuilder, FormGroup, Validators} from "@angular/forms";
import {LoginDto} from "../../interfaces/Utilities";
import {environment} from "../../../../environments/environment";

@Component({
  selector: 'app-login-card',
  templateUrl: './loginCard.component.html',
})
export class LoginCardComponent {
  invalidCredentials = false;
  redirectUrl: string | null = null;

  @Output() switchToRegister = new EventEmitter<void>();

  constructor(private tokenService: TokenService, private router: Router) {
    this.loginForm.controls.email.setValue(environment.userEmailAddress);
    this.loginForm.controls.password.setValue(environment.userPassword);
    this.subscribeToPasswordFieldChanged();
  }


  redirectToRegister() {
    this.switchToRegister.emit();
  }

  readonly loginForm = new FormGroup({
    email: new FormBuilder().control('', [Validators.required, Validators.email]),
    password: new FormBuilder().control('', [Validators.required])
  });

  async login(): Promise<void> {
    try {
      await this.tokenService.login(this.loginForm.value as LoginDto);
      if (this.tokenService.isAuthenticated()) {
        await this.router.navigate([this.redirectUrl || '/dashboard']);
      }
    } catch (e : any) {
      if (e.message === 'Invalid credentials') {
        this.invalidCredentials = true;
      }
    }
  }

  subscribeToPasswordFieldChanged(): void {
    const passwordControl: AbstractControl = this.loginForm.get('password')!;

    if (passwordControl) {
      passwordControl.valueChanges.subscribe((value) => {
        this.invalidCredentials = false;
      });
    }
  }
}
