import {HttpClient} from '@angular/common/http';
import {Injectable} from "@angular/core";
import {firstValueFrom} from "rxjs";
import {environment} from "../../../environments/environment";
import {
  CalendarDayMetricDto, CalendarDayMetricViewDto,
  MetricRegisterMetricDto,
  MetricValueViewDto, MetricViewDto
} from "../interfaces/dtos/metric.dto.interface";
import {CalendarDay, CalendarDayMetric} from "../interfaces/day.interface";

@Injectable({
  providedIn: 'root'
})
export class MetricService {
  private apiUrl = environment.baseUrl + '/metric';

  constructor(private http: HttpClient) {
  }

  public async getAllMetricsWithValues(): Promise<MetricViewDto[]> {
    const call = this.http.get<MetricViewDto[]>(`${this.apiUrl}/values`);
    return await firstValueFrom(call);
  }

  public async getMetricsForDay(date: Date): Promise<CalendarDayMetricViewDto[]> {
    const call = this.http.get<CalendarDayMetricViewDto[]>(`${this.apiUrl}/${date.toISOString()}`);
    return await firstValueFrom(call);
  }

  public async addMetricsForDay(date: string, metrics: MetricRegisterMetricDto[]){
    const call = this.http.post(`${this.apiUrl}?dateTimeOffsetString=${date}`, metrics);
    return await firstValueFrom(call);
  }

  public async getMetricsForCalendarDays(startDate: Date, endDate: Date): Promise<CalendarDayMetric[]> {
    const call = this.http.get<CalendarDayMetric[]>(`${this.apiUrl}/calendar?startDate=${startDate.toISOString()}&endDate=${endDate.toISOString()}`);
    const data = await firstValueFrom(call);
    console.log(data);
    return data;
  }
}
