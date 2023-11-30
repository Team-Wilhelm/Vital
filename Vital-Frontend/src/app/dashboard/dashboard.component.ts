import {AfterViewInit, Component, ElementRef, OnDestroy, OnInit, ViewChild} from '@angular/core';
import {CycleService} from "../services/cycle.service";
import {Router} from "@angular/router";
import {MetricService} from "../services/metric.service";
import {CalendarDayMetric, CycleDay} from "../interfaces/day.interface";
import {DataService} from "../services/data.service";
import {Subscription} from "rxjs";
import {CalendarComponent} from "../calendar/calendar.component";

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html'
})

export class DashboardComponent implements OnInit, OnDestroy {
  @ViewChild('calendarComponent') calendarComponent: CalendarComponent | undefined;
  private readonly dateSubscription: Subscription;
  private readonly metricDeletedSubscription: Subscription;
  private readonly metricAddedSubscription: Subscription;

  clickedDate = new Date();

  title = 'dashboard';
  nextPeriodInDays: number = 0;
  currentCycleDays: CycleDay[] = [];

  constructor(public cycleService: CycleService, public metricService: MetricService, public dataService: DataService, private router: Router) {
    cycleService.getPredictedPeriod().then(() => {
      this.nextPeriodInDays = this.calculateNextPeriodInDays();
    });

    this.dateSubscription = this.dataService.clickedDate$.subscribe(clickedDate => {
      if (clickedDate) {
        this.updateDashboardData(clickedDate);
        this.clickedDate = clickedDate;
      }
    });

    this.metricDeletedSubscription = this.metricService.metricDeleted$.subscribe(metricDeleted => {
      if (metricDeleted) {
        this.updateCalendar();
      }
    });

    this.metricAddedSubscription = this.metricService.newMetricAdded$.subscribe(newMetricAdded => {
      if (newMetricAdded) {
        this.updateCalendar();
      }
    });
  }

  ngOnInit() {

  }

  ngOnDestroy() {
    this.dateSubscription.unsubscribe();
    this.metricDeletedSubscription.unsubscribe();
    this.metricAddedSubscription.unsubscribe();
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
    this.metricService.getUsersMetric(date);
    // TODO: Update calendar events when a metric is deleted
  }

  updateCalendar() {
    this.calendarComponent && this.calendarComponent.getPeriodDays();
    this.calendarComponent && this.calendarComponent.getPredictedPeriodDays();
    this.metricService.setMetricDeleted(false);
    this.metricService.setNewMetricAdded(false);
  }
}
