import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class DataService {
  private clickedDateSource = new BehaviorSubject<Date | null>(null);
  clickedDate$ = this.clickedDateSource.asObservable();
  clickedDate: Date | null = null;

  constructor() {
    this.setClickedDate(new Date());
  }

  setClickedDate(clickedDate: Date) {
    this.clickedDateSource.next(clickedDate);
    this.clickedDate = clickedDate;
  }

  getCurrentUTCTime() : Date {
    // Get the current local time and convert it to UTC
    const currentDate = new Date();
    const localDate = new Date(this.clickedDateSource.getValue()!);
    localDate.setHours(currentDate.getHours(), currentDate.getMinutes(), currentDate.getSeconds(), currentDate.getMilliseconds());

    // Create a new Date object, representing the same instant in time
    const dateWithCurrentTime = new Date(localDate);
    // Set the time using UTC methods
    dateWithCurrentTime.setUTCHours(localDate.getUTCHours(), localDate.getUTCMinutes(), localDate.getUTCSeconds(), localDate.getUTCMilliseconds());

    return dateWithCurrentTime;
  }

  getCurrentLocalTime() : Date {
    const currentDate = new Date();
    const localDate = new Date(this.clickedDateSource.getValue()!);
    localDate.setHours(currentDate.getHours(), currentDate.getMinutes(), currentDate.getSeconds(), currentDate.getMilliseconds());

    return localDate;
  }

  getUTCDate(date: Date) : Date {
    const utcDate = new Date(date);
    utcDate.setUTCHours(date.getUTCHours(), date.getUTCMinutes(), date.getUTCSeconds(), date.getUTCMilliseconds());

    return utcDate;
  }
}
