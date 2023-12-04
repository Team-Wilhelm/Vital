import {Component, EventEmitter, OnDestroy, Output} from "@angular/core";
import {AbstractControl, FormBuilder, FormGroup, Validators} from "@angular/forms";
import {UserSessionService} from "../../services/user-session.service";
import {Router} from "@angular/router";
import {PasswordValidator} from "../../validators/password.validator";
import {PasswordRules, RegisterDto} from "../../interfaces/Utilities";
import {debounceTime, distinctUntilChanged, Subscription} from "rxjs";
import {em} from "@fullcalendar/core/internal-common";

@Component({
  selector: 'app-register-card',
  templateUrl: './registerCard.component.html',
})
export class RegisterCardComponent implements OnDestroy {
  @Output() switchToLogin = new EventEmitter<void>();

  private passwordSubscription: Subscription | undefined;
  private emailSubscription: Subscription | undefined;

  passwordVisible = false;

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

  constructor(private tokenService: UserSessionService, private router: Router) {
    this.subscribeToPasswordChanges();
    this.subscribeToEmailChanges();
  }

  ngOnDestroy(): void {
    this.passwordSubscription?.unsubscribe();
    this.emailSubscription?.unsubscribe();
  }

  async register(): Promise<void> {
    if (this.registerForm.invalid) return;
    await this.tokenService.register(this.registerForm.value as RegisterDto);
  }

  passwordMatchValidator(control: AbstractControl) {
    const g = control as FormGroup;
    return g.get('password')?.value === g.get('repeatPassword')?.value
      ? null : {'mismatch': true};
  }

  subscribeToPasswordChanges(): void {
    const passwordControl: AbstractControl = this.registerForm.get('password')!;

    if (passwordControl) {
      this.passwordSubscription = passwordControl.valueChanges.subscribe((value) => {
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

  subscribeToEmailChanges(): void {
    const emailControl: AbstractControl = this.registerForm.get('email')!;

    // If the user has not typed in anything for 500ms, check if the username is taken
    if (emailControl) {
      this.emailSubscription = emailControl.valueChanges.pipe(
        debounceTime(500),
        distinctUntilChanged()
      ).subscribe((value) => {
        this.checkIfUsernameIsTaken(value);
      });
    }
  }

  redirectToLogin() {
    this.switchToLogin.emit();
  }

  checkIfUsernameIsTaken(email: string): void {
    if (email === null || email === '' || email === undefined) return;

    this.tokenService.checkIfUsernameIsTaken(email)
      .then(r => {
        if (r) {
          this.registerForm.get('email')?.setErrors({usernameTaken: true});
        }
      });
  }

  getEmailErrorMessage(): string {
    const emailControl: AbstractControl = this.registerForm.get('email')!;
    if (emailControl.hasError('required')) {
      return '';
    } else if (emailControl.hasError('email')) {
      return 'Please enter a valid email address';
    } else if (emailControl.hasError('usernameTaken')) {
      return 'This username is already taken';
    }
    return '';
  }

  togglePasswordVisibility() {
    this.passwordVisible = !this.passwordVisible;
  }
}
