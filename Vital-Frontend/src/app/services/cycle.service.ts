import {Injectable} from "@angular/core";
import {HttpClient} from "@angular/common/http";
import {firstValueFrom} from "rxjs";
import {environment} from "../../../environments/environment";
import {Cycle} from "../interfaces/cycle.interface";

@Injectable({
  providedIn: 'root'
})

export class CycleService {
  currentCycle: Cycle | undefined;
  predictedPeriod: Date[] = [];

  constructor(private httpClient: HttpClient) {
    this.getPredictedPeriod();
  }

  async getPredictedPeriod() {
    this.predictedPeriod = await firstValueFrom(this.httpClient.get<Date[]>(environment.baseUrl + '/cycle/predicted-period'));
    this.predictedPeriod = this.predictedPeriod.map(date => new Date(date));
  }

  startNewCycle() {
    return firstValueFrom(this.httpClient.post(environment.baseUrl + '/cycle', {}));
  }
}
