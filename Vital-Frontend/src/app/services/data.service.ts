import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class DataService {
  private clickedDateSource = new BehaviorSubject<Date | null>(null);
  clickedDate$ = this.clickedDateSource.asObservable();

  setClickedDate(clickedDate: Date) {
    this.clickedDateSource.next(clickedDate);
    console.log('Clicked date changed to: ' + clickedDate.toISOString());
  }
}
