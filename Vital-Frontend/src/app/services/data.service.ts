import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class DataService {
  private clickedDateSource = new BehaviorSubject<Date | null>(null);
  clickedDate$ = this.clickedDateSource.asObservable();
  clickedDate: Date = new Date();

  setClickedDate(clickedDate: Date) {
    this.clickedDateSource.next(clickedDate);
    this.clickedDate = clickedDate;
  }
}
