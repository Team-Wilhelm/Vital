import {Injectable} from "@angular/core";
import {environment} from "../../../environments/environment";
import {firstValueFrom} from "rxjs";
import {InitialLoginGetDto, InitialLoginPostDto} from "../interfaces/dtos/user.dto.interface";
import {HttpClient} from "@angular/common/http";
import {aW} from "@fullcalendar/core/internal-common";
import {ResetPasswordDto} from "../interfaces/account/resetPasswordDto.interface";
import {ForgotPasswordDto} from "../interfaces/account/forgotPasswordDto.interface";
import {VerifyRequestDto} from "../interfaces/account/verifyEmailDto.interface";

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

  public async verifyEmail(dto: VerifyRequestDto): Promise<void> {
    const request = this.httpClient.post(environment.baseUrl + '/Account/Verify-Email', dto);
    await firstValueFrom(request);
  }

  public async forgotPassword(dto: ForgotPasswordDto): Promise<void> {
    const request = this.httpClient.post(environment.baseUrl + '/Account/Forgot-Password', dto);
    await firstValueFrom(request);
  }

  public async resetPassword(dto: ResetPasswordDto): Promise<void> {
    const request = this.httpClient.post(environment.baseUrl + '/Account/Reset-Password', dto);
    await firstValueFrom(request);
  }
}
