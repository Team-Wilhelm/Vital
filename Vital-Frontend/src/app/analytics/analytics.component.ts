import {Component, OnInit, ViewEncapsulation} from '@angular/core';
import Chart from 'chart.js/auto';
import {CycleService} from "../services/cycle.service";
import {CycleAnalyticsDto, PeriodCycleStatsDto} from '../interfaces/analytics.interface';


@Component({
  selector: 'app-analytics',
  templateUrl: './analytics.component.html',
  styleUrls: ['./analytics.component.css'],
  encapsulation: ViewEncapsulation.None
})
export class AnalyticsComponent implements OnInit {
  public analytics: CycleAnalyticsDto[] = [];
  public allCycleDays: number[] = [];
  public allPeriodDays: number[] = [];
  public allNonPeriodDays: number[] = [];
  public periodCycleStats: PeriodCycleStatsDto | undefined;
  public selectedOption: number = 3
  public options: number[] = [3, 6, 9, 12];
  public chart?: Chart | null;

  constructor(private cycleService: CycleService) {

  }

  async createChart(numberOfCycles: number) {
    this.analytics = await this.cycleService.getAnalytics(numberOfCycles);

    this.allCycleDays = this.getTotalCycleDays(this.analytics);
    this.allPeriodDays = this.getTotalPeriodDays(this.analytics);
    this.allNonPeriodDays = this.getNonPeriodDays(this.analytics);

    if (this.chart) {
      this.chart.destroy();
    }

    const chart = new Chart('myChart', {
      type: 'bar',
      data: {
        labels: this.getCycleLabels(),
        datasets: [
          {
            label: "Period",
            data: this.allPeriodDays,
            backgroundColor: [
              'rgb(246, 203, 209, 0.2)'
            ],
            borderColor: [
              'rgb(246, 203, 209, 1)'
            ],
            borderWidth: 2
          },
          {
            label: "Non period",
            data: this.allNonPeriodDays,
            backgroundColor: [
              'rgb(112, 172, 199, 0.2)'
            ],
            borderColor: [
              'rgb(112, 172, 199, 1)'
            ],
            borderWidth: 2
          }
        ]
      },
      options: {
        indexAxis: 'y',
        responsive: true,
        scales: {
          x: {
            stacked: true,
          },
          y: {
            stacked: true
          }
        }
      }
    });

    this.chart = chart;
  }

  async ngOnInit(): Promise<void> {
    await this.createChart(3);
    this.periodCycleStats = await this.cycleService.getUserStats();
  }

  getTotalCycleDays(cycleAnalytics: CycleAnalyticsDto[]) {
    return cycleAnalytics.map(a => {
      const endTime = a.endDate.getTime();
      const startTime = a.startDate.getTime();

      const timeDiff = endTime - startTime;

      // The number of milliseconds in one day
      const oneDay = 1000 * 60 * 60 * 24;

      // Convert time difference into days
      const daysBetween = Math.ceil(timeDiff / oneDay);

      return daysBetween;
    });
  }

  getTotalPeriodDays(cycleAnalytics: CycleAnalyticsDto[]) {
    return cycleAnalytics.map(a => a.periodDays.length);
  }

  getNonPeriodDays(cycleAnalytics: CycleAnalyticsDto[]) {
    return cycleAnalytics.map(a => {

      const endTime = a.endDate.getTime();

      // Convert periodDays length into milliseconds
      const periodDaysMilliseconds = a.periodDays.length * 24 * 60 * 60 * 1000;

      const startTime = a.startDate.getTime();

      const totalCycleDaysMilliseconds = endTime - startTime;

      // Calculate non-period days in milliseconds
      const nonPeriodDaysMilliseconds = totalCycleDaysMilliseconds - periodDaysMilliseconds;

      // The number of milliseconds in one day
      const oneDay = 24 * 60 * 60 * 1000;

      // Convert milliseconds into days
      const nonPeriodDays = Math.ceil(nonPeriodDaysMilliseconds / oneDay);

      return nonPeriodDays;
    });
  }

  async onOptionsSelected(item: number) {
    this.selectedOption = item;
    await this.createChart(item);
  }

  getCurrentCycleLength(): number {
    return this.periodCycleStats?.currentCycleLength ?? 0;
  }

  getAverageCycleLength(): number {
    return this.periodCycleStats?.averageCycleLength ?? 0;
  }

  getAveragePeriodLength(): number {
    return this.periodCycleStats?.averagePeriodLength ?? 0;
  }

  getCycleLabels(): string[] {
    return this.analytics.map(a => a.startDate.toLocaleDateString());
  }
}


