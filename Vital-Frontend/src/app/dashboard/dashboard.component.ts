import {AfterViewInit, Component, ElementRef, ViewChild} from '@angular/core';
import {CycleService} from "../services/cycle.service";
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
    nextPeriodInDays: number = 0;
    @ViewChild('hasYourPeriodStartedModal') hasYourPeriodStartedModal!: ElementRef;
    currentCycleDays: CycleDay[] = [];
    selectedDay: Date = new Date(); // TODO: Set value of selected day based on selected day in calendar
    selectedDayMetrics: CalendarDayMetric[] = [];

  constructor(public cycleService: CycleService, private metricService: MetricService) {
    cycleService.getPredictedPeriod().then(() => {
      this.nextPeriodInDays = this.calculateNextPeriodInDays();
    });
  }

  private calculateNextPeriodInDays() {
    this.cycleService.predictedPeriod.sort((a, b) => a.getTime() - b.getTime());
    const today = new Date();
    let nextPeriodIndex = this.cycleService.predictedPeriod.findIndex(date => date.getTime() > today.getTime());
    if (nextPeriodIndex === -1) {
      nextPeriodIndex = 0;
    }
    const nextPeriod = this.cycleService.predictedPeriod[nextPeriodIndex];
    const diffTime = Math.abs(nextPeriod.getTime() - today.getTime());
    return Math.ceil(diffTime / (1000 * 60 * 60 * 24));
  }
    async ngOnInit() {
        this.selectedDay.setDate(this.selectedDay.getDate() - 1);
        await this.getMetricsForDay(this.selectedDay);
    }

  public displayHasYourPeriodStartedDialog() : void {
    this.hasYourPeriodStartedModal.nativeElement.showModal();
    // Focus on yes button
    setTimeout(() => {
      const yesButton = document.getElementById('yes-button');
      yesButton?.focus();
    }, 100);
  }
    //TODO: Get the metrics for selected day
    async getMetricsForDay(date: Date) {
        this.selectedDayMetrics = (await this.metricService.getMetricsForCalendarDays(this.selectedDay, new Date()))[0].selectedMetrics;
        console.log(this.selectedDayMetrics);
    }
}
