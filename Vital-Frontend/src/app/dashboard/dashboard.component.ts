import {Component, OnDestroy, OnInit, ViewChild} from '@angular/core';
import {CycleService} from "../services/cycle.service";
import {Router} from "@angular/router";
import {MetricService} from "../services/metric.service";
import {CycleDay} from "../interfaces/day.interface";
import {DataService} from "../services/data.service";
import {Subscription} from "rxjs";
import {CalendarComponent} from "../calendar/calendar.component";
import {ToastService} from "../services/toast.service";

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

  constructor(public cycleService: CycleService,
              public metricService: MetricService,
              public dataService: DataService,
              private toastService: ToastService,
              private router: Router) {
    cycleService.getPredictedPeriod().then(() => {
      this.nextPeriodInDays = this.calculateNextPeriodInDays();
    });

    this.dateSubscription = this.dataService.clickedDate$.subscribe(clickedDate => {
      if (clickedDate) {
        this.updateDashboardData(clickedDate).then();
        this.clickedDate = clickedDate;
      }
    });

    this.metricDeletedSubscription = this.metricService.metricDeleted$.subscribe(metricDeleted => {
      if (metricDeleted) {
        this.showToast('Metric deleted', 'The metric was successfully deleted', 'success');
        this.updateCalendar().then();
      }
    });

    this.metricAddedSubscription = this.metricService.newMetricAdded$.subscribe(newMetricAdded => {
      if (newMetricAdded) {
        this.showToast('Metric added', 'The metric was successfully added', 'success');
        this.updateCalendar().then();
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

  async redirectToMetrics() {
    await this.router.navigate(['/add-metric']);
  }

  async updateDashboardData(date: Date) {
    // Call the methods to update your dashboard data here
    await this.metricService.getUsersMetric(date);
  }

  async updateCalendar() {
    this.calendarComponent && await this.calendarComponent.updateCalendar();
    this.metricService.setMetricDeleted(false);
    this.metricService.setNewMetricAdded(false);
  }

  showToast(title: string, message: string, type: 'info' | 'success' | 'error') {
    this.toastService.show(message, title, type);
  }
}
