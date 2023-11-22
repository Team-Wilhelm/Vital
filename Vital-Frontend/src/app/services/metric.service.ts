import {HttpClient} from '@angular/common/http';
import {Injectable} from "@angular/core";
import {Metric, MetricDto} from "../add-metric-page/add-metric-page.component";
import {firstValueFrom} from "rxjs";

@Injectable({
  providedIn: 'root'
})
export class MetricService {
  private apiUrl = 'something/metrics'; //TODO: Add API URL
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
