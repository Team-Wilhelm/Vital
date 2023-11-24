import {HttpClient} from '@angular/common/http';
import {Injectable} from "@angular/core";
import {firstValueFrom} from "rxjs";
import {environment} from "../../../environments/environment";
import {
  CalendarDayMetricViewDto,
  MetricRegisterMetricDto,
  MetricViewDto
} from "../interfaces/dtos/metric.dto.interface";
import {CalendarDay} from "../interfaces/day.interface";

@Injectable({
  providedIn: 'root'
})
export class MetricService {
  private apiUrl = environment.baseUrl + '/metric';
  allMetrics: MetricViewDto[] = [];
  metricSelectionMap: Map<string, string | null> = new Map(); // <MetricId, MetricValueId>
  selectedMetrics: MetricRegisterMetricDto[] = [];
  periodMetric: MetricViewDto | undefined;

  constructor(private http: HttpClient) {
    this.getAllMetricsWithValues();
    this.getUsersMetric(new Date());
  }

  public async getAllMetricsWithValues(): Promise<MetricViewDto[]> {
    const call = this.http.get<MetricViewDto[]>(`${this.apiUrl}/values`);
    this.allMetrics = (await firstValueFrom(call)).filter((metric) => metric.name !== "Flow");
    this.periodMetric = (await firstValueFrom(call)).filter((metric) => metric.name === "Flow")[0];
    return this.allMetrics;
  }

  public async getUsersMetric(date: Date): Promise<void> {
    const  calendarDayArray =  await firstValueFrom(this.http.get<CalendarDayMetricViewDto[]>(`${this.apiUrl}/${date.toISOString()}`));
    calendarDayArray.forEach((calendarDay) => {
      this.metricSelectionMap.set(calendarDay.metricsId, calendarDay.metricValueId || null);
    });
    console.log(this.metricSelectionMap);
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
    // Get date from route params
    const date = new Date().toISOString();

    // Add selected metrics to the selectedMetrics array
    this.selectedMetrics = [];
    this.metricSelectionMap.forEach((value, key) => {
        this.selectedMetrics.push({
          metricsId: key,
          metricValueId: value ? value : undefined
      });
    });

    console.log(this.selectedMetrics);

    this.http.post(`${this.apiUrl}?date=${date}`, this.selectedMetrics).subscribe((response) => {
      console.log(response);
    });
  }

  async getPeriodDays(previousMonthFirstDay: Date, thisMonthLastDay: Date) {
    const call = this.http.get<Date[]>(`${this.apiUrl}/period?fromDate=${previousMonthFirstDay.toISOString()}&toDate=${thisMonthLastDay.toISOString()}`);
    return await firstValueFrom(call);
  }
}
