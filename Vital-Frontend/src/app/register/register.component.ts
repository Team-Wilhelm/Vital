import {Component} from "@angular/core";
import {AbstractControl, FormBuilder, FormGroup, Validators} from "@angular/forms";
import {TokenService} from "../services/token.service";
import {Router} from "@angular/router";
import {PasswordValidator} from "../validators/password.validator";

@Component({
    selector: 'app-register',
    templateUrl: './register.component.html',
})
export class RegisterComponent {
    readonly registerForm = new FormGroup({
        email: new FormBuilder().control('', [Validators.required, Validators.email]),
        password: new FormBuilder().control('', [Validators.required, PasswordValidator]),
        repeatPassword: new FormBuilder().control('', [Validators.required, PasswordValidator])
    }, {validators: this.passwordMatchValidator});

    constructor(private tokenService: TokenService, private router: Router) {
    }

    async register(): Promise<void> {
        if (this.registerForm.invalid) return;
    }

    passwordMatchValidator(control: AbstractControl) {
        const g = control as FormGroup;
        return g.get('password')?.value === g.get('repeatPassword')?.value
            ? null : {'mismatch': true};
    }
}
