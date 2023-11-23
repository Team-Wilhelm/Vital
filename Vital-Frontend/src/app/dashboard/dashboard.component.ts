import {Component} from '@angular/core';
import {MetricService} from "../services/metric.service";
import {CalendarDayMetric, CycleDay} from "../interfaces/day.interface";
import {Metrics} from "../interfaces/metric.interface";
import {CycleService} from "../services/cycle.service";

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html'
})

export class DashboardComponent {
  title = 'dashboard';
  currentCycleDays: CycleDay[] = [];
  selectedDay: Date = new Date();
  selectedDayMetrics: CalendarDayMetric[] = [];

  constructor(private cycleService: CycleService, private metricService: MetricService) {
    var d = new Date();
    d.setDate(d.getDate() - 1);
    this.getMetricsForDay(d).then(
        (res => {
            console.log(res);
        })
    );
  }

  async getMetricsForDay(date: Date) {
    this.selectedDay = date;

    this.selectedDayMetrics = await this.metricService.getMetricsForCalendarDays(date, date);
  }
}
