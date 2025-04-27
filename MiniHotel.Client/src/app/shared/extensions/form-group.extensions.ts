/**
 * Adds FormGroup.normalizeDates().
 * Converts Date controls to plain `yyyy-MM-dd` strings
 * so they reach the API without UTC-shift.
 *
 * Usage → form.normalizeDates(['startDate','endDate']);
*/

import { FormGroup } from '@angular/forms';
import { formatDateOnly } from '../utils/date.utils';

export type DateFields = string[];

declare module '@angular/forms' {
    interface FormGroup {
        /* Normalizes the specified date fields to yyyy-MM-dd */
        normalizeDates(fields: DateFields): void;
    }
}

(FormGroup.prototype as any).normalizeDates = function (
    this: FormGroup,
    fields: DateFields
): void {
    for (const field of fields) {
        const ctrl = this.get(field);
        const value = ctrl?.value;
        if (value instanceof Date) {
            ctrl?.setValue(formatDateOnly(value));
        }
    }
}