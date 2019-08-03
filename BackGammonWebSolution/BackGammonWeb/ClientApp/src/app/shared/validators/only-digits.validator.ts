import { AbstractControl, ValidatorFn } from '@angular/forms';

export function onlyDigitsValidator(): ValidatorFn {
  return (control: AbstractControl): {[key: string]: any} | null => {
    if (control.value==="") {
      return null;
    }
    const onlyDigits = !/^[a-z0-9]+$/.test(control.value);
    return onlyDigits ? {'onlyDigits': {value: control.value}} : null;
  };
}
