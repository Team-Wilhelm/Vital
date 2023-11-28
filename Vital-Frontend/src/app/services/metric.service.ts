import {HttpClient} from '@angular/common/http';
import {Injectable, OnDestroy, OnInit} from "@angular/core";
import {firstValueFrom, map, Subscription} from "rxjs";
import {environment} from "../../../environments/environment";
import {
  MetricRegisterMetricDto,
  MetricViewDto
} from "../interfaces/dtos/metric.dto.interface";
import {CalendarDay, CalendarDayMetric} from "../interfaces/day.interface";
import {DataService} from "./data.service";

@Injectable({
  providedIn: 'root'
})
export class MetricService implements OnDestroy {
  private apiUrl = environment.baseUrl + '/metric';
  private readonly subscription: Subscription | undefined;

  clickedDate = new Date();

  allMetrics: MetricViewDto[] = [];
  metricSelectionMap: Map<string, MetricSelection> = new Map(); // <MetricId, MetricValueId>
  loggedMetrics: CalendarDayMetric[] = [];
  periodMetric: MetricViewDto | undefined;

  constructor(private http: HttpClient, private dataService: DataService) {
    this.getAllMetricsWithValues();
    this.getUsersMetric(this.clickedDate);

    this.subscription = this.dataService.clickedDate$.subscribe(clickedDate => {
      if (clickedDate) {
        this.clickedDate = clickedDate;
        this.getUsersMetric(clickedDate);
      }
    });
  }

  ngOnDestroy(): void {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }

  public async getAllMetricsWithValues(): Promise<MetricViewDto[]> {
    const call = this.http.get<MetricViewDto[]>(`${this.apiUrl}/values`);
    this.allMetrics = (await firstValueFrom(call)).filter((metric) => metric.name !== "Flow");
    this.periodMetric = (await firstValueFrom(call)).filter((metric) => metric.name === "Flow")[0];
    return this.allMetrics;
  }

  public async getUsersMetric(date: Date): Promise<void> {
    // Format date as 'YYYY-MM-DD'
    let formattedDate = date.getFullYear() + '-' + String(date.getMonth() + 1).padStart(2, '0') + '-' + String(date.getDate()).padStart(2, '0');

    // Add the local offset to the date in +HH:mm format (e.g. +02:00)
    const offset = date.getTimezoneOffset();
    const offsetSign = offset < 0 ? '+' : '-';
    const offsetHours = Math.abs(Math.floor(offset / 60));
    const offsetMinutes = Math.abs(offset % 60);
    formattedDate += offsetSign + String(offsetHours).padStart(2, '0') + ':' + String(offsetMinutes).padStart(2, '0');

    // Get the metrics for the date
    this.metricSelectionMap.clear();
    const calendarDayArray = await firstValueFrom(this.http.get<CalendarDayMetric[]>(`${this.apiUrl}/${formattedDate}`));
    calendarDayArray.forEach((calendarDay) => {
      calendarDay.createdAt = new Date(calendarDay.createdAt);

      this.metricSelectionMap.set(
        calendarDay.metricsId,
        {
          metricId: calendarDay.metricsId,
          metricValueId: calendarDay.metricValueId || null,
          createdAt: calendarDay.createdAt
        });
    });
    this.loggedMetrics = calendarDayArray;
  }

  public async getMetricsForCalendarDays(startDate: Date, endDate: Date): Promise<CalendarDay[]> {
    const call = this.http.get<CalendarDay[]>(`${this.apiUrl}/calendar?fromDate=${startDate.toISOString()}&toDate=${endDate.toISOString()}`);
    return await firstValueFrom(call);
  }

  addOrRemoveMetric(metric: MetricViewDto) {
    // Check if the metric is already selected, if not, add it
    if (!this.metricSelectionMap.has(metric.id)) {
      // Add the metric to the selected metrics
      this.metricSelectionMap.set(metric.id, {
        metricId: metric.id,
        metricValueId: null,
        createdAt: this.dataService.getCurrentUTCTime()
      });
    } else {
      // Remove the metric from the selected metrics
      this.metricSelectionMap.delete(metric.id);
    }
  }

  updateMetricTime(metricId: string, metricTime: Date) {
    // Check if the metric is already selected, if not, exit
    if (!this.metricSelectionMap.has(metricId)) {
      return;
    }

    const createdAtUTCDate = this.dataService.getUTCDate(metricTime);
    this.metricSelectionMap.set(metricId, {
      metricId: metricId,
      metricValueId: this.metricSelectionMap.get(metricId)?.metricValueId || null,
      createdAt: createdAtUTCDate
    });
  }

  selectOptionalValue(metricId: string, optionalValueId: string) {

    const createdAt = this.metricSelectionMap.get(metricId)!.createdAt;
    if (this.metricSelectionMap.get(metricId)?.metricValueId === optionalValueId) {
      // If the optional value is already selected, deselect it
      this.metricSelectionMap.set(metricId, {
        metricId: metricId,
        metricValueId: null,
        createdAt: createdAt!
      });
      return;
    }

    this.metricSelectionMap.set(metricId, {
      metricId: metricId,
      metricValueId: optionalValueId,
      createdAt: createdAt!
    });
    console.log("Value selected: " + optionalValueId);
  }

  isMetricSelected(metricId: string) {
    return this.metricSelectionMap.has(metricId);
  }

  getSelectedOptionalValue(metricId: string) {
    const valueId = this.metricSelectionMap.get(metricId)?.metricValueId;
    if (!valueId) {
      return "Optional";
    }

    if (metricId === this.periodMetric?.id) {
      return this.periodMetric?.values.filter((value) => value.id === valueId)[0].name;
    }

    const value = this.allMetrics.filter((metric) =>
      metric.id === metricId)[0].values.filter((value) => value.id === valueId)[0];
    return value.name;
  }

  saveMetrics() {
    // Add selected metrics to the selectedMetrics array
    const metricsToPost = [] as MetricRegisterMetricDto[];
    this.metricSelectionMap.forEach((value, key) => {
      metricsToPost.push({
        metricsId: key,
        metricValueId: value.metricValueId ? value.metricValueId : undefined,
        createdAt: value.createdAt
      });
    });

    this.http.post(`${this.apiUrl}`, metricsToPost)
      .subscribe(() => {
        this.getUsersMetric(this.clickedDate);
      });
  }

  async getPeriodDays(previousMonthFirstDay: Date, nextMonthLastDay: Date) {
    const call = this.http.get<Date[]>(`${this.apiUrl}/period?fromDate=${previousMonthFirstDay.toISOString()}&toDate=${nextMonthLastDay.toISOString()}`);
    const result = await firstValueFrom(call);
    return result.map(date => new Date(date));
  }
}

export interface MetricSelection {
  metricId: string;
  metricValueId: string | null;
  createdAt: Date;
}
