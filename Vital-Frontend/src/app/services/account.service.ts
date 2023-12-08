import {Injectable} from "@angular/core";
import {environment} from "../../../environments/environment";
import {firstValueFrom, tap} from "rxjs";
import {InitialLoginGetDto, InitialLoginPostDto} from "../interfaces/dtos/user.dto.interface";
import {HttpClient} from "@angular/common/http";
import {aW} from "@fullcalendar/core/internal-common";
import {ResetPasswordDto} from "../interfaces/account/resetPasswordDto.interface";
import {ForgotPasswordDto} from "../interfaces/account/forgotPasswordDto.interface";
import {VerifyRequestDto} from "../interfaces/account/verifyEmailDto.interface";
import {ToastService} from "./toast.service";
import {Router} from "@angular/router";
import HttpService from "./http.service";
import {addWarning} from "@angular-devkit/build-angular/src/utils/webpack-diagnostics";

@Injectable({
  providedIn: 'root'
})
export default class AccountService {
  constructor(private httpClient: HttpClient, private toastService: ToastService, private router: Router, private httpService: HttpService) {
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
    await this.httpService.post('/Identity/Account/Verify-Email', dto, 'Email Verified');

    await this.router.navigateByUrl('/')
  }

  public async forgotPassword(dto: ForgotPasswordDto): Promise<void> {
    await this.httpService.post('/Identity/Account/Forgot-Password', dto, 'Password reset link sent, Check your email for a password reset link')

    await this.router.navigateByUrl('/')
  }

  public async resetPassword(dto: ResetPasswordDto): Promise<void> {
    await this.httpService.post('/Identity/Account/Reset-Password', dto, 'Your password was successfully reset');

    await this.router.navigateByUrl('/')
  }
}
