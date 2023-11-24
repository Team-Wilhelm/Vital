import {HttpClient} from '@angular/common/http';
import {Injectable} from "@angular/core";
import {firstValueFrom} from "rxjs";
import {environment} from "../../../environments/environment";
import {
  CalendarDayMetricViewDto,
  CalendarDayMetricDto,
  MetricRegisterMetricDto,
  MetricViewDto,
  MetricValueViewDto
} from "../interfaces/dtos/metric.dto.interface";
import {CalendarDay, CycleDay} from "../interfaces/day.interface";
import {CycleDayDto} from "../interfaces/dtos/day.dto.interface";

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

  //TODO: Look into casting the retrieved data into another type
  public async getMetricsForCalendarDays(startDate: Date, endDate: Date): Promise<CalendarDay[]> {
    const call = this.http.get<CalendarDay[]>(`${this.apiUrl}/calendar?fromDate=${startDate.toISOString()}&toDate=${endDate.toISOString()}`);
    return await firstValueFrom(call);
  }

  async getPeriodDays(previousMonthFirstDay: Date, thisMonthLastDay: Date) {
    const call = this.http.get<Date[]>(`${this.apiUrl}/period?fromDate=${previousMonthFirstDay.toISOString()}&toDate=${thisMonthLastDay.toISOString()}`);
    return await firstValueFrom(call);
  }
}
