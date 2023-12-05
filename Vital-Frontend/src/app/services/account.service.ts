import {Injectable} from "@angular/core";
import {environment} from "../../../environments/environment";
import {firstValueFrom} from "rxjs";
import {InitialLoginGetDto, InitialLoginPostDto} from "../interfaces/dtos/user.dto.interface";
import {HttpClient} from "@angular/common/http";

@Injectable({
  providedIn: 'root'
})
export default class AccountService {
  constructor(private httpClient: HttpClient) {
  }

  public async setInitialLoginData(loginData: InitialLoginPostDto): Promise<void> {
    try {
      const request = this.httpClient.put(environment.baseUrl + '/cycle/initial-login', loginData);
      await firstValueFrom(request);
    } catch (e) {
      throw new Error('Could not set initial login data.');
    }
  }

  public async checkIfUsernameIsTaken(username: string): Promise<boolean> {
    const request = this.httpClient.get<boolean>(environment.baseUrl + '/identity/auth/username-taken/' + username);
    return await firstValueFrom(request);
  }

  public async isFirstLogin() {
    const request = this.httpClient.get<InitialLoginGetDto>(environment.baseUrl + '/cycle/initial-login');
    const response = await firstValueFrom(request);
    return response.periodLength === null || response.cycleLength === null;
  }
}
