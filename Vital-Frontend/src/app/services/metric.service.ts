import {HttpClient} from '@angular/common/http';
import {Injectable, OnDestroy, OnInit} from "@angular/core";
import {firstValueFrom, Subscription} from "rxjs";
import {environment} from "../../../environments/environment";
import {
  CalendarDayMetricViewDto,
  MetricRegisterMetricDto,
  MetricViewDto
} from "../interfaces/dtos/metric.dto.interface";
import {CalendarDay, CalendarDayMetric} from "../interfaces/day.interface";
import {DataService} from "./data.service";

@Injectable({
  providedIn: 'root'
})
export class MetricService implements OnInit, OnDestroy {
  private apiUrl = environment.baseUrl + '/metric';
  private subscription: Subscription | undefined;

  clickedDate = new Date();

  allMetrics: MetricViewDto[] = [];
  metricSelectionMap: Map<string, string | null> = new Map(); // <MetricId, MetricValueId>
  loggedMetrics: CalendarDayMetric[] = [];
  periodMetric: MetricViewDto | undefined;

  constructor(private http: HttpClient, private dataService: DataService) {
    this.getAllMetricsWithValues();
    this.getUsersMetric(this.clickedDate);
  }

  ngOnInit(): void {
    this.subscription = this.dataService.clickedDate$.subscribe(clickedDate => {
      if (clickedDate) {
        this.clickedDate = clickedDate;
        this.getUsersMetric(clickedDate);
        console.log("Clicked date changed to: " + clickedDate);
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
    // Format date as 'YYYY-MM-DD' in local timezone
    const formattedDate = date.getFullYear() + '-' + String(date.getMonth() + 1).padStart(2, '0') + '-' + String(date.getDate()).padStart(2, '0');
    console.log("Getting metrics for date: " + formattedDate);
    const calendarDayArray = await firstValueFrom(this.http.get<CalendarDayMetric[]>(`${this.apiUrl}/${formattedDate}`));
    calendarDayArray.forEach((calendarDay) => {
      this.metricSelectionMap.set(calendarDay.metricsId, calendarDay.metricValueId || null);
    });
    this.loggedMetrics = calendarDayArray;
  }

  //TODO: Look into casting the retrieved data into another type
  public async getMetricsForCalendarDays(startDate: Date, endDate: Date): Promise<CalendarDay[]> {
    const call = this.http.get<CalendarDay[]>(`${this.apiUrl}/calendar?fromDate=${startDate.toISOString()}&toDate=${endDate.toISOString()}`);
    return await firstValueFrom(call);
  }

  addOrRemoveMetric(metric: MetricViewDto) {
    if (!this.metricSelectionMap.has(metric.id)) {
      // Add the metric to the selected metrics
      this.metricSelectionMap.set(metric.id, null);
    } else {
      // Remove the metric from the selected metrics
      this.metricSelectionMap.delete(metric.id);
    }
    console.log(this.metricSelectionMap);
  }

  selectOptionalValue(metricId: string, optionalValueId: string) {
    // Check if the metric is already selected, if not, exit
    if (!this.metricSelectionMap.has(metricId)) {
      return;
    }

    if (this.metricSelectionMap.get(metricId) === optionalValueId) {
      // If the optional value is already selected, deselect it
      this.metricSelectionMap.set(metricId, null);
      return;
    }

    this.metricSelectionMap.set(metricId, optionalValueId);
    console.log("Value selected: " + optionalValueId);
  }

  isMetricSelected(metricId: string) {
    return this.metricSelectionMap.has(metricId);
  }

  getSelectedOptionalValue(metricId: string) {
    const valueId = this.metricSelectionMap.get(metricId);
    if (!valueId) {
      return "Optional";
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
        metricValueId: value ? value : undefined
      });
    });

    this.http.post(`${this.apiUrl}?date=${this.clickedDate.toISOString()}`, metricsToPost)
      .subscribe(() => {
        this.getUsersMetric(this.clickedDate);
      });
  }

  async getPeriodDays(previousMonthFirstDay: Date, thisMonthLastDay: Date) {
    const call = this.http.get<Date[]>(`${this.apiUrl}/period?fromDate=${previousMonthFirstDay.toISOString()}&toDate=${thisMonthLastDay.toISOString()}`);
    return await firstValueFrom(call);
  }
}
