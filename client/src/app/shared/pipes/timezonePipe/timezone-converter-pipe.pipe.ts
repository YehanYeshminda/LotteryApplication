import { Pipe, PipeTransform } from '@angular/core';
import * as moment from 'moment-timezone';

@Pipe({
  name: 'timezoneConverter'
})
export class TimezoneConverterPipe implements PipeTransform {
  transform(utcTime: string, timeZone: string): string {
    if (!utcTime || !timeZone) {
      return '';
    }

    const localTime = moment.utc(utcTime).tz(timeZone).format('YYYY-MM-DD hh:mm:ss A');
    return localTime;
  }
}
