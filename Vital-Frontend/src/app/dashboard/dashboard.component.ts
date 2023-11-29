import {AfterViewInit, Component, ElementRef, OnDestroy, OnInit, ViewChild} from '@angular/core';
import {CycleService} from "../services/cycle.service";
import {Router} from "@angular/router";
import {MetricService} from "../services/metric.service";
import {CalendarDayMetric, CycleDay} from "../interfaces/day.interface";
import {DataService} from "../services/data.service";
import {Subscription} from "rxjs";

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html'
})

export class DashboardComponent implements OnInit, OnDestroy {
  private subscription: Subscription | undefined;
  clickedDate = new Date();

  title = 'dashboard';
  nextPeriodInDays: number = 0;
  currentCycleDays: CycleDay[] = [];

  constructor(public cycleService: CycleService, public metricService: MetricService, public dataService: DataService, private router: Router) {
    cycleService.getPredictedPeriod().then(() => {
      this.nextPeriodInDays = this.calculateNextPeriodInDays();
    });
  }

  ngOnInit() {
    this.dataService.clickedDate$.subscribe(clickedDate => {
      if (clickedDate) {
        this.updateDashboardData(clickedDate);
        this.clickedDate = clickedDate;
      }
    });
  }

  ngOnDestroy() {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
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

  redirectToMetrics() {
    this.router.navigate(['/add-metric']);
  }

  updateDashboardData(date: Date) {
    // Call the methods to update your dashboard data here
    // For example:
    this.metricService.getUsersMetric(date);
    // Add other methods as needed
  }
}
