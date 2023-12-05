import {Injectable} from "@angular/core";
import {environment} from "../../../environments/environment";
import {firstValueFrom} from "rxjs";
import {ApplicationUserInitialLoginDto} from "../interfaces/dtos/user.dto.interface";
import {HttpClient} from "@angular/common/http";

@Injectable({
  providedIn: 'root'
})
export default class AccountService {

  constructor(private httpClient: HttpClient) {
  }

  public async setInitialLoginData(periodLength: number, cycleLength: number,  periodStart: Date, periodEnd?: Date): Promise<void> {
    const request = this.httpClient.put(environment.baseUrl + '/identity/account/initial-login', {
      periodLength: periodLength,
      cycleLength: cycleLength,
      periodStart: periodStart,
      periodEnd: periodEnd
    });
    await firstValueFrom(request);
  }

  public async checkIfUsernameIsTaken(username: string): Promise<boolean> {
    const request = this.httpClient.get<boolean>(environment.baseUrl + '/identity/auth/username-taken/' + username);
    return await firstValueFrom(request);
  }

  public async isFirstLogin() {
    const request = this.httpClient.get<ApplicationUserInitialLoginDto>(environment.baseUrl + '/identity/account/initial-login');
    const response = await firstValueFrom(request);
    console.log(response);
    return response.periodLength === null || response.cycleLength === null;
  }
}
