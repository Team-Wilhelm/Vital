import {HttpClient} from '@angular/common/http';
import {Injectable} from "@angular/core";
import {firstValueFrom} from "rxjs";
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
export class MetricService {
  private apiUrl = environment.baseUrl + '/metric';
  private clickedDate: Date = new Date();

  allMetrics: MetricViewDto[] = [];

  metricSelectionMap: Map<string, string | null> = new Map(); // <MetricId, MetricValueId>
  loggedMetrics: CalendarDayMetric[] = [];
  periodMetric: MetricViewDto | undefined;

  constructor(private http: HttpClient, private dataService: DataService) {
    this.dataService.clickedDate$.subscribe((clickedDate) => {
      // When the date changes, update the selected metrics for the new date
      if (clickedDate) {
        this.clickedDate = clickedDate;
      }
    });

    this.getAllMetricsWithValues();
    this.getUsersMetric(this.clickedDate);
  }

  public async getAllMetricsWithValues(): Promise<MetricViewDto[]> {
    const call = this.http.get<MetricViewDto[]>(`${this.apiUrl}/values`);
    this.allMetrics = (await firstValueFrom(call)).filter((metric) => metric.name !== "Flow");
    this.periodMetric = (await firstValueFrom(call)).filter((metric) => metric.name === "Flow")[0];
    return this.allMetrics;
  }

  public async getUsersMetric(date: Date): Promise<void> {
    const  calendarDayArray =  await firstValueFrom(this.http.get<CalendarDayMetric[]>(`${this.apiUrl}/${date.toISOString()}`));
    calendarDayArray.forEach((calendarDay) => {
      this.metricSelectionMap.set(calendarDay.metricsId, calendarDay.metricValueId || null);
    });
    this.loggedMetrics = calendarDayArray;
    console.log(this.loggedMetrics);
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
    const metricsToPost =  [] as MetricRegisterMetricDto[];
    this.metricSelectionMap.forEach((value, key) => {
      metricsToPost.push({
          metricsId: key,
          metricValueId: value ? value : undefined
      });
    });

    this.http.post(`${this.apiUrl}?date=${this.dataService.clickedDate.toISOString()}`, metricsToPost).subscribe(() => {
      this.getUsersMetric(this.dataService.clickedDate);
    });
  }

  async getPeriodDays(previousMonthFirstDay: Date, thisMonthLastDay: Date) {
    const call = this.http.get<Date[]>(`${this.apiUrl}/period?fromDate=${previousMonthFirstDay.toISOString()}&toDate=${thisMonthLastDay.toISOString()}`);
    return await firstValueFrom(call);
  }
}
