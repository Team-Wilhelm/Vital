import {Component, EventEmitter, Output} from "@angular/core";
import {AbstractControl, FormBuilder, FormGroup, Validators} from "@angular/forms";
import {TokenService} from "../../services/token.service";
import {Router} from "@angular/router";
import {PasswordValidator} from "../../validators/password.validator";
import {PasswordRules, RegisterDto} from "../../interfaces/Utilities";

@Component({
  selector: 'app-register-card',
  templateUrl: './registerCard.component.html',
})
export class RegisterCardComponent {
  @Output() switchToLogin = new EventEmitter<void>();

  readonly registerForm = new FormGroup({
    email: new FormBuilder().control('', [Validators.required, Validators.email]),
    password: new FormBuilder().control('', [Validators.required, PasswordValidator]),
    repeatPassword: new FormBuilder().control('', [Validators.required, PasswordValidator])
  }, {validators: this.passwordMatchValidator});

  passwordRulesMet: PasswordRules = {
    lengthCondition: false,
    digitCondition: false,
    lowercaseCondition: false,
    uppercaseCondition: false,
    specialCondition: false
  };

  constructor(private tokenService: TokenService, private router: Router) {
    this.subscribeToPasswordChanges();
  }

  async register(): Promise<void> {
    if (this.registerForm.invalid) return;
    await this.tokenService.register(this.registerForm.value as RegisterDto);
    await this.router.navigate(['/']);
  }

  passwordMatchValidator(control: AbstractControl) {
    const g = control as FormGroup;
    return g.get('password')?.value === g.get('repeatPassword')?.value
      ? null : {'mismatch': true};
  }

  subscribeToPasswordChanges(): void {
    const passwordControl: AbstractControl = this.registerForm.get('password')!;

    if (passwordControl) {
      passwordControl.valueChanges.subscribe((value) => {
        this.passwordRulesMet = {
          lengthCondition: value.length >= 6,
          digitCondition: /\d/.test(value),
          lowercaseCondition: /[a-z]/.test(value),
          uppercaseCondition: /[A-Z]/.test(value),
          specialCondition: /[^a-zA-Z\d]/.test(value)
        };
      });
    }
  }
  redirectToLogin() {
    this.switchToLogin.emit();
  }

}
