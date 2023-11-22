import {HttpClient} from '@angular/common/http';
import {Injectable} from "@angular/core";
import {Metric, MetricDto} from "../interfaces/metric";
import {firstValueFrom} from "rxjs";
import {environment} from "../../../environments/environment";

@Injectable({
  providedIn: 'root'
})
export class MetricService {
  private apiUrl = environment.baseUrl + '/metrics';
  constructor(private http: HttpClient) {
  }

  public async getAllMetricsWithValues(): Promise<Metric[]> {
    const call = this.http.get<Metric[]>(`${this.apiUrl}`);
    return await firstValueFrom(call);
  }

  public async getMetricsForDay(date: string): Promise<Metric[]> {
    const call = this.http.get<Metric[]>(`${this.apiUrl}/${date}`);
    return await firstValueFrom(call);
  }
  public async addMetricsForDay(date: string, metrics: MetricDto[]){
    const call = this.http.post(`${this.apiUrl}/metrics/${date}`, metrics);
    return await firstValueFrom(call);
  }
}
