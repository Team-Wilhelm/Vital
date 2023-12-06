import {Component} from "@angular/core";
import {AbstractControl, FormControl, FormGroup, Validators} from "@angular/forms";
import AccountService from "../services/account.service";
import {InitialLoginPostDto} from "../interfaces/dtos/user.dto.interface";
import {Router} from "@angular/router";

@Component({
  selector: 'first-login',
  template: `
      <div class="flex flex-col items-center justify-center h-full w-full">
          <h1 class="text-4xl font-bold text-center z-30">Welcome to Vital</h1>
          <p class="text-center z-30">We are so glad to have you!</p>
          <p class="text-center mb-3 z-30">Before you can start using the app, we need you to answer a few quick
              questions.</p>

          <div class="grid grid-rows-4 grid-cols-2 gap-3 z-30">

              <!-- Average cycle length -->
              <p class="col-span-1 row-span-1 flex items-center h-full">What is the average length of your cycles?</p>
              <input class="col-span-1 row-span-1 input input-bordered " type="number" min="1" max="100"
                     [formControl]="formGroup.controls.cycleLength"
                     (keydown)="numericInput($event)"
                     placeholder="Average cycle length">

              <!-- Average period length -->
              <p class="col-span-1 row-span-1 flex items-center h-full">What is the average length of your periods?</p>
              <input class="col-span-1 row-span-1 input input-bordered" type="number" min="1" max="100"
                     [formControl]="formGroup.controls.periodLength"
                     (keydown)="numericInput($event)"
                     placeholder="Average period length">

              <!-- Last period start -->
              <div class="flex flex-col">
                  <p class="col-span-1 row-span-1 flex items-center h-full">When did your last period start?</p>
                  <p class="text-error text-xs z-30"
                     *ngIf="formGroupHasStartInFutureError()">Period cannot start in the future</p>
              </div>
              <input class="col-span-1 row-span-1 input input-bordered w-full z-30" type="date"
                     [formControl]="formGroup.controls.lastPeriodStart" placeholder="Start of last period"
                     [class.input-error]="formGroupHasStartInFutureError()">

              <!-- Last period end -->
              <div class="flex flex-col">
                  <p class="col-span-1 row-span-1 flex items-center h-full">When did your last period end?</p>
                  <p class="text-xs"
                     [class.text-error]="formGroupHasEndInFutureError() || formGroupHasEndBeforeStartError()">{{ getEndDateHintText() }}</p>
              </div>
              <input class="col-span-1 row-span-1 input input-bordered w-full" type="date"
                     [formControl]="formGroup.controls.lastPeriodEnd" placeholder="End of last period"
                     [class.input-error]="formGroupHasEndBeforeStartError() || formGroupHasEndInFutureError()"
              >

              <!-- Submit button -->
              <button class="col-span-2 row-span-1 btn btn-primary mt-2" [disabled]="formGroup.invalid"
                      (click)="submit()">Save
              </button>
          </div>
      </div>
  `
})
export class FirstLoginComponent {
  title = 'first-login';
  formGroup = new FormGroup({
    cycleLength: new FormControl(28, [Validators.required, Validators.min(1), Validators.pattern('^(0?[1-9]|[1-9][0-9]|100)$')]),
    periodLength: new FormControl(5, [Validators.required, Validators.min(1), Validators.pattern('^(0?[1-9]|[1-9][0-9]|100)$')]),
    lastPeriodStart: new FormControl('', [Validators.required]),
    lastPeriodEnd: new FormControl(''),
  }, {validators: [this.periodDatesValidator, this.periodStartInFutureValidator, this.periodEndInFutureValidator]});

  constructor(private accountService: AccountService, private router: Router) {
    const today = new Date();
    this.formGroup.controls.lastPeriodStart.setValue(today.toISOString().split('T')[0]);

    this.formGroup.controls.lastPeriodStart.valueChanges
      .subscribe(value => {
        const periodStartControl = this.formGroup.controls.lastPeriodStart;
        const periodEndControl = this.formGroup.controls.lastPeriodEnd;
        const periodStartDate = new Date(periodStartControl.value!);

        if (periodStartDate > today) {
          periodEndControl?.disable();
          periodEndControl?.setValue('');
        } else {
          periodEndControl?.enable();
        }
      });

  }

  periodDatesValidator(control: AbstractControl) {
    const formGroup = control as FormGroup;
    const periodStart = formGroup.get('lastPeriodStart')?.value;
    const periodEnd = formGroup.get('lastPeriodEnd')?.value;

    if (periodStart && periodEnd) {
      return periodStart < periodEnd
        ? null
        : {periodEndBeforePeriodStart: true};
    }
    return null;
  }

  periodStartInFutureValidator(control: AbstractControl) {
    const formGroup = control as FormGroup;
    const today = new Date();
    const periodStart = formGroup.get('lastPeriodStart')?.value;

    if (periodStart && new Date(periodStart) > today) {
      return {periodStartInFuture: true};
    }

    return null;
  }

  periodEndInFutureValidator(control: AbstractControl) {
    const formGroup = control as FormGroup;
    const today = new Date();
    const periodEnd = formGroup.get('lastPeriodEnd')?.value;

    if (periodEnd && new Date(periodEnd) > today) {
      return {periodEndInFuture: true};
    }

    return null;
  }

  async submit() {
    if (this.formGroup.invalid) {
      return;
    }
    const formGroupControls = this.formGroup.controls;
    const loginData = {
      periodLength: formGroupControls.periodLength.value,
      cycleLength: formGroupControls.cycleLength.value,
      lastPeriodStart: new Date(formGroupControls.lastPeriodStart.value!),
      lastPeriodEnd: formGroupControls.lastPeriodEnd.value === '' ? null : new Date(formGroupControls.lastPeriodEnd.value!)
    } as InitialLoginPostDto;

    try {
      await this.accountService.setInitialLoginData(loginData);
      await this.router.navigate(['/dashboard']);
    } catch (e) {
      console.log(e);
    }
  }

  numericInput(event: KeyboardEvent) {
    if (event.key === 'ArrowUp' || event.key === 'ArrowDown' || event.key === 'ArrowLeft' || event.key === 'ArrowRight' ||
      event.key === 'Backspace' || event.key === 'Delete' || event.key === 'Tab' || event.key === 'Enter') {
      return;
    }

    if (event.key === '0' || event.key === '1' || event.key === '2' || event.key === '3' || event.key === '4' ||
      event.key === '5' || event.key === '6' || event.key === '7' || event.key === '8' || event.key === '9') {
      return;
    }
    event.preventDefault();
  }

  formGroupHasEndBeforeStartError(): boolean {
    return this.formGroup.hasError('periodEndBeforePeriodStart');
  }

  formGroupHasStartInFutureError(): boolean {
    return this.formGroup.hasError('periodStartInFuture');
  }

  formGroupHasEndInFutureError(): boolean {
    return this.formGroup.hasError('periodEndInFuture');
  }

  getEndDateHintText() {
    if (this.formGroupHasEndBeforeStartError()) {
      return 'End date cannot be before the start date';
    } else if (this.formGroupHasEndInFutureError()) {
      return 'End date cannot be in the future';
    } else {
      return 'Leave empty if it hasn\'t ended yet';
    }
  }
}
