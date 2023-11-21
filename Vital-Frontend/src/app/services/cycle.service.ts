import {Injectable} from "@angular/core";
import {Cycle, CycleDay} from "../interfaces/Models";
import {HttpClient} from "@angular/common/http";
import {firstValueFrom} from "rxjs";
import {environment} from "../../../environments/environment";

@Injectable({
  providedIn: 'root'
})

export class CycleService {
  currentCycleDays: CycleDay[] = [];
  lastThreeCycles: Cycle[] = [];
  currentCycleWeek: CycleDay[] = []; // 3 days before and after current day
  currentCycle: Cycle | undefined;

  constructor(private httpClient: HttpClient) {
    this.getCurrentCycleFromApi();
  }

  async getCurrentCycleDays(): Promise<CycleDay[]> {
    await this.getCurrentCycleFromApi();
    return this.currentCycleDays;
  }

  getLastThreeCycles(): Cycle[] {
    return this.lastThreeCycles;
  }

  getCurrentCycleWeek(): CycleDay[] {
    return this.currentCycleWeek;
  }

  async getCurrentCycleFromApi() {
    this.currentCycle = await firstValueFrom(this.httpClient.get<Cycle>(environment.baseUrl + '/cycle/2af6bc6c-b3c0-4e77-97d9-9fa6d36c4a0a'));
    this.currentCycleDays = this.currentCycle.cycleDays;

    // I am doing this because the date is coming from the API as a UTC string
    // and javascript is not converting it to a date object automatically
    this.currentCycleDays.forEach((day, index) => {
      day.date = new Date(day.date);
    });

    // Sort the days by date
    this.currentCycleDays.sort((a, b) => a.date.getTime() - b.date.getTime());

    // There should be 7 days in the current cycle, but three of them are in the future, so we need to add three cycle days
    // they should later be replaced with predictions for the cycle
    for (let i = 1; i <= 3; i++) {
      const dummyDay: CycleDay = {
        id: 'dummy-id-' + i, // You can replace this with a real GUID generator
        date: new Date(new Date().setDate(new Date().getDate() + i)),
        UserId: 'dummy-user-id', // Replace with a real user ID
        user: {
          id: 'dummy-user-id', // Replace with a real user ID
          name: 'Dummy User',
          email: 'dummy@user.com',
          passwordHash: 'dummyPasswordHash',
          role: 'User'
        },
        metrics: [], // You can add dummy metrics if needed
        state: 'dummyState',
        cycleId: 'dummy-cycle-id', // Replace with a real cycle ID
        cycle: {
          id: 'dummy-cycle-id', // Replace with a real cycle ID
          startDate: new Date(new Date().setDate(new Date().getDate() + i + 1)),
          endDate: new Date(new Date(this.currentCycleDays[this.currentCycleDays.length - 1].date)
            .setDate(this.currentCycleDays[this.currentCycleDays.length - 1].date.getDate() + i + 1)),
          userId: 'dummy-user-id', // Replace with a real user ID
          user: {
            id: 'dummy-user-id', // Replace with a real user ID
            name: 'Dummy User',
            email: 'dummy@user.com',
            passwordHash: 'dummyPasswordHash',
            role: 'User'
          },
          cycleDays: [], // You can add dummy cycle days if needed
          period: undefined
        },
        isPeriod: false
      };

      this.currentCycleDays.push(dummyDay);

      // If there are not enough days in the cycle, add the last few days from the previous cycle
    }
    console.log(this.currentCycleDays);
  }
}
