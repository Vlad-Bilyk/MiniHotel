import { ValidatorFn, AbstractControl, ValidationErrors } from '@angular/forms';

export const dateRangeValidator: ValidatorFn = (
    formGroup: AbstractControl
): ValidationErrors | null => {
    const start = formGroup.get('startDate')?.value;
    const end = formGroup.get('endDate')?.value;

    if (start && end && end < start) {
        return { dateRange: 'Дата виїзду має бути не раніше дати заїзду' };
    }

    return null;
};
