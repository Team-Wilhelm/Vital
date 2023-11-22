import {Injectable} from "@angular/core";
import {Cycle, CycleDay} from "../interfaces/Models";
import {HttpClient} from "@angular/common/http";
import {firstValueFrom} from "rxjs";
import {environment} from "../../../environments/environment";

@Injectable({
  providedIn: 'root'
})

export class CycleService {
  currentCycle: Cycle | undefined;

  constructor(private httpClient: HttpClient) {
    this.getCurrentCycleFromApi();
  }

  async getCurrentCycleFromApi() {
    this.currentCycle = await firstValueFrom(this.httpClient.get<Cycle>(environment.baseUrl + '/cycle/current-cycle'));

    if (this.currentCycle) {
      this.currentCycle.cycleDays.forEach((cycleDay: CycleDay) => {
        cycleDay.date = new Date(cycleDay.date);
      });
    }

    // Find out what's the last saved day of the current cycle
    const lastDay = this.currentCycle.cycleDays.reduce((prev, current) => (prev.date > current.date) ? prev : current);


  }
}
