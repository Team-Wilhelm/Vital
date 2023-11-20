import {AbstractControl} from "@angular/forms";

export function PasswordValidator(control: AbstractControl)  {
    const value = control.value;

    const lengthCondition: boolean = value.length >= 6;
    const digitCondition: boolean = /\d/.test(value);
    const lowercaseCondition: boolean = /[a-z]/.test(value);
    const uppercaseCondition: boolean = /[A-Z]/.test(value);
    const specialCondition: boolean = /[^a-zA-Z\d]/.test(value);

    const valid = lengthCondition && digitCondition && lowercaseCondition && uppercaseCondition && specialCondition;
    if (valid) return null;

    return {validPassword: false};
}
