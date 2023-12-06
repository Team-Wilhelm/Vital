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

@Injectable({
  providedIn: 'root'
})
export default class AccountService {
  constructor(private httpClient: HttpClient, private toastService: ToastService, private router: Router) {
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
    try {
      const response = await firstValueFrom(this.httpClient.post(environment.baseUrl + '/Identity/Account/Verify-Email', dto, {observe: 'response'}));
      if (response.status === 200) {
        this.toastService.show('Email verified', 'Your email was successfully verified', 'success', 5000);
      }
    } catch (error:any) {
      this.toastService.show(error.error.detail, 'Error', 'error', 5000);
    }
    await this.router.navigateByUrl('/')
  }

  public async forgotPassword(dto: ForgotPasswordDto): Promise<void> {
    try {
      const response = await firstValueFrom(this.httpClient.post(environment.baseUrl + '/Identity/Account/Forgot-Password', dto, {observe: 'response'}));
      if (response.status === 200) {
        this.toastService.show('Password reset link sent', 'Check your email for a password reset link', 'success', 5000);
      }
    } catch (error:any) {
      this.toastService.show(error.error.detail, 'Error', 'error', 5000);
    }
    await this.router.navigateByUrl('/')
  }

  public async resetPassword(dto: ResetPasswordDto): Promise<void> {
    try {
      const response = await firstValueFrom(this.httpClient.post(environment.baseUrl + '/Identity/Account/Reset-Password', dto, {observe: 'response'}));
      if (response.status === 200) {
        this.toastService.show('Password reset', 'Your password was successfully reset', 'success', 5000);
      }
    } catch (error:any) {
      this.toastService.show(error.error.detail, 'Error', 'error', 5000);
    }
    await this.router.navigateByUrl('/')
  }
}
