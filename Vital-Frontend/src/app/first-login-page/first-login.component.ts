import {Component} from "@angular/core";
import {AbstractControl, FormControl, FormGroup, Validators} from "@angular/forms";

@Component({
  selector: 'first-login',
  template: `
      <div class="flex flex-col items-center justify-center h-full w-full">
          <h1 class="text-4xl font-bold text-center z-50">Welcome to Vital</h1>
          <p class="text-center z-50">We are so glad to have you!</p>
          <p class="text-center mb-3 z-50">Before you can start using the app, we need you to answer a few quick
              questions.</p>

          <div class="grid grid-rows-3 grid-cols-2 gap-3 z-50">
              <p class="col-span-1 row-span-1 flex items-center h-full">What is the average length of your cycles?</p>
              <input class="col-span-1 row-span-1 input input-bordered " type="number"
                     [formControl]="formGroup.controls.cycleLength"
                     placeholder="Average cycle length">
              <p class="col-span-1 row-span-1 flex items-center h-full">What is the average length of your periods?</p>
              <input class="col-span-1 row-span-1 input input-bordered" type="number"
                     [formControl]="formGroup.controls.periodLength"
                     placeholder="Average period length">
              <p class="col-span-1 row-span-1 flex items-center h-full">When did your last period start?</p>
              <input class="col-span-1 row-span-1 input input-bordered" type="date"
                     [formControl]="formGroup.controls.lastPeriodStart" placeholder="Start of last period">
              <p class="col-span-1 row-span-1 flex items-center h-full">When did your last period end?</p>
              <div class="relative">
                  <input class="col-span-1 row-span-1 input input-bordered relative w-full" type="date"
                         [formControl]="formGroup.controls.lastPeriodEnd" placeholder="End of last period"
                         [class.input-error]="formGroup.hasError('periodEndBeforePeriodStart')">
                  <p class="absolute left-0 right-0 -bottom-6 text-error text-center" *ngIf="formGroup.hasError('periodEndBeforePeriodStart')">End date cannot be before
                      the start date</p>
              </div>
          </div>
      </div>
  `
})
export class FirstLoginComponent {
  title = 'first-login';
  formGroup = new FormGroup({
    cycleLength: new FormControl('', [Validators.required, Validators.min(1)]),
    periodLength: new FormControl('', [Validators.required, Validators.min(1)]),
    lastPeriodStart: new FormControl('', [Validators.required]),
    lastPeriodEnd: new FormControl(''),
  }, {validators: this.periodEndAfterPeriodStartValidator});

  constructor() {
  }

  periodEndAfterPeriodStartValidator(control: AbstractControl) {
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
}
