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
  metricSelectionMap: Map<string, string> = new Map(); // <MetricId, MetricValueId>
  selectedMetrics: MetricRegisterMetricDto[] = [];
  periodMetric: MetricViewDto | undefined;

  constructor(private http: HttpClient) {
    this.getAllMetricsWithValues();


  }

  public async getAllMetricsWithValues(): Promise<MetricViewDto[]> {
    const call = this.http.get<MetricViewDto[]>(`${this.apiUrl}/values`);
    this.allMetrics = (await firstValueFrom(call)).filter((metric) => metric.name !== "Flow");
    return this.allMetrics;
  }

  public async getMetricsForDay(date: Date): Promise<CalendarDayMetricViewDto[]> {
    const call = this.http.get<CalendarDayMetricViewDto[]>(`${this.apiUrl}/${date.toISOString()}`);
    return await firstValueFrom(call);
  }

  public async addMetricsForDay(date: string, metrics: MetricRegisterMetricDto[]) {
    const call = this.http.post(`${this.apiUrl}?dateTimeOffsetString=${date}`, metrics);
    return await firstValueFrom(call);
  }

  //TODO: Look into casting the retrieved data into another type
  public async getMetricsForCalendarDays(startDate: Date, endDate: Date): Promise<CalendarDay[]> {
    const call = this.http.get<CalendarDay[]>(`${this.apiUrl}/calendar?fromDate=${startDate.toISOString()}&toDate=${endDate.toISOString()}`);
    return await firstValueFrom(call);
  }

   addOrRemoveMetric(metric: MetricViewDto, adding: boolean, event: MouseEvent) {
    if (event.target !== event.currentTarget) event.preventDefault(); // Prevents the click event from propagating to the parent element, otherwise deselecting optional values would deselect the metric as well

    if (adding) {
      // Add the metric to the selected metrics
      this.metricSelectionMap.set(metric.id, '');
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
      this.metricSelectionMap.set(metricId, '');
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


  public updateSelectedMetrics() {
    // Clear the selectedMetrics array
    this.selectedMetrics = [];

    // Add selected metrics to the selectedMetrics array
    this.allMetrics.forEach((metric) => {
      const selection = this.metricSelectionMap.get(metric.id);
      if (selection) {
        this.selectedMetrics.push({
          metricsId: metric.id,
          metricValueId: selection,
        });
      }
    });
  }
}
