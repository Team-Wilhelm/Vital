import {Injectable} from "@angular/core";
import {HttpClient} from "@angular/common/http";
import {firstValueFrom} from "rxjs";
import {environment} from "../../../environments/environment";
import {CycleDay} from "../interfaces/day.interface";
import {Cycle} from "../interfaces/cycle.interface";

@Injectable({
  providedIn: 'root'
})

export class CycleService {
  currentCycleDays: CycleDay[] = [];
  lastThreeCycles: Cycle[] = [];
  currentCycleWeek: CycleDay[] = []; // 3 days before and after current day
  currentCycle: Cycle | undefined;

  constructor(private httpClient: HttpClient) {

  }
}
