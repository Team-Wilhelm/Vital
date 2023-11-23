import {HttpClient} from '@angular/common/http';
import {Injectable} from "@angular/core";
import {firstValueFrom} from "rxjs";
import {environment} from "../../../environments/environment";
import {
  CalendarDayMetricDto, CalendarDayMetricViewDto,
  MetricRegisterMetricDto,
  MetricValueViewDto, MetricViewDto
} from "../interfaces/dtos/metric.dto.interface";

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

  public async getMetricsForDay(date: string): Promise<CalendarDayMetricViewDto[]> {
    const call = this.http.get<CalendarDayMetricViewDto[]>(`${this.apiUrl}/${date}`);
    return await firstValueFrom(call);
  }
  public async addMetricsForDay(date: string, metrics: MetricRegisterMetricDto[]){
    const call = this.http.post(`${this.apiUrl}?dateTimeOffsetString=${date}`, metrics);
    return await firstValueFrom(call);
  }
}
