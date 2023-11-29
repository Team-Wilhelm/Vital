import {Component, EventEmitter, Output} from "@angular/core";
import {TokenService} from "../../services/token.service";
import {Router} from "@angular/router";
import {FormBuilder, FormGroup, Validators} from "@angular/forms";
import {LoginDto} from "../../interfaces/Utilities";
import {environment} from "../../../../environments/environment";

@Component({
    selector: 'app-login-card',
    templateUrl: './loginCard.component.html',
})
export class LoginCardComponent {
    redirectUrl: string | null = null;

    @Output() switchToRegister = new EventEmitter<void>();

    redirectToRegister() {
      this.switchToRegister.emit();
    }
    readonly loginForm = new FormGroup({
        email: new FormBuilder().control('', [Validators.required, Validators.email]),
        password: new FormBuilder().control('', [Validators.required])
    });

    constructor(private tokenService: TokenService, private router: Router) {
        this.loginForm.controls.email.setValue(environment.userEmailAddress);
        this.loginForm.controls.password.setValue(environment.userPassword);
    }

    async login(): Promise<void> {
        await this.tokenService.login(this.loginForm.value as LoginDto);
        if (this.tokenService.isAuthenticated()) {
            await this.router.navigate([this.redirectUrl || '/dashboard']);
        }
    }

    // TODO: Add link to register page
    // TODO: Nicer login page for mobile, add background image
}
