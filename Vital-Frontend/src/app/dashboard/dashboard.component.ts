import {Component} from '@angular/core';
import {CycleDay} from "../interfaces/Models";
import {CycleService} from "../services/cycle.service";

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html'
})

export class DashboardComponent {
  title = 'dashboard';
  currentCycleDays: CycleDay[] = [];

  constructor(private cycleService: CycleService) {
    this.initCurrentCycleDays();
  }

  async initCurrentCycleDays() {
   /* const days = await this.cycleService.getCurrentCycleDays();
    this.currentCycleDays = days.filter(day => {
      const today = new Date();
      today.setHours(0, 0, 0, 0); // remove time part
      const dayDate = new Date(day.date);
      dayDate.setHours(0, 0, 0, 0); // remove time part

      const threeDaysBefore = new Date(today.getTime() - 3 * 24 * 60 * 60 * 1000);
      const threeDaysAfter = new Date(today.getTime() + 3 * 24 * 60 * 60 * 1000);

      return dayDate >= threeDaysBefore && dayDate <= threeDaysAfter;
    });
    console.log(this.currentCycleDays);*/
  }
}
