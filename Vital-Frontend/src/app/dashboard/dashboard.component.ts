import {Component, OnInit} from '@angular/core';
import {MetricService} from "../services/metric.service";
import {CalendarDayMetric, CycleDay} from "../interfaces/day.interface";
import {Metrics} from "../interfaces/metric.interface";
import {CycleService} from "../services/cycle.service";

@Component({
    selector: 'app-dashboard',
    templateUrl: './dashboard.component.html'
})

export class DashboardComponent implements OnInit {
    title = 'dashboard';
    currentCycleDays: CycleDay[] = [];
    selectedDay: Date = new Date(); // TODO: Set value of selected day based on selected day in calendar
    selectedDayMetrics: CalendarDayMetric[] = [];

    constructor(private cycleService: CycleService, private metricService: MetricService) {

    }

    async ngOnInit() {
        this.selectedDay.setDate(this.selectedDay.getDate() - 1);
        await this.getMetricsForDay(this.selectedDay);
    }

    //TODO: Get the metrics for selected day
    async getMetricsForDay(date: Date) {
        this.selectedDayMetrics = (await this.metricService.getMetricsForCalendarDays(this.selectedDay, new Date()))[0].selectedMetrics;
        console.log(this.selectedDayMetrics);
    }
}
