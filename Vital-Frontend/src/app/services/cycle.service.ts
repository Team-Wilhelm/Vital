import {Injectable} from "@angular/core";
import {HttpClient} from "@angular/common/http";
import {firstValueFrom} from "rxjs";
import {environment} from "../../../environments/environment";
import {Cycle} from "../interfaces/cycle.interface";
import {CycleAnalyticsDto, PeriodCycleStatsDto} from "../interfaces/analytics.interface";

@Injectable({
  providedIn: 'root'
})

export class CycleService {
  currentCycle: Cycle | undefined;
  predictedPeriod: Date[] = [];

  constructor(private httpClient: HttpClient) {
    this.getPredictedPeriod().then();
  }

  async getPredictedPeriod() {
    this.predictedPeriod = await firstValueFrom(this.httpClient.get<Date[]>(environment.baseUrl + '/cycle/predicted-period'));
    const today = new Date(new Date().setHours(0, 0, 0, 0));
    this.predictedPeriod = this.predictedPeriod.map(date => new Date(date)).filter(date => date >= today);
  }

  async getAnalytics(numberOfCycles: number) {
    const result = await firstValueFrom(this.httpClient.get<CycleAnalyticsDto[]>(environment.baseUrl + '/cycle/analytics/' + numberOfCycles));
    result.forEach(cycle => {
      cycle.startDate = new Date(cycle.startDate);
      cycle.endDate = cycle.endDate ? new Date(cycle.endDate) : new Date();
      cycle.periodDays = cycle.periodDays.map(day => new Date(day));
    });
    result.sort((a, b) => b.startDate.getTime() - a.startDate.getTime()); // sort from oldest to newest
    return result;
  }

  async getUserStats() {
    return firstValueFrom(this.httpClient.get<PeriodCycleStatsDto>(environment.baseUrl + '/cycle/period-cycle-stats'));
  }

  startNewCycle() {
    return firstValueFrom(this.httpClient.post(environment.baseUrl + '/cycle', {}));
  }
}
