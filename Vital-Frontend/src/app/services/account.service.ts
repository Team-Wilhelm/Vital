import {Injectable} from "@angular/core";
import {environment} from "../../../environments/environment";
import {firstValueFrom} from "rxjs";
import {InitialLoginGetDto, InitialLoginPostDto} from "../interfaces/dtos/user.dto.interface";
import {HttpClient} from "@angular/common/http";
import {ResetPasswordDto} from "../interfaces/account/resetPasswordDto.interface";
import {ForgotPasswordDto} from "../interfaces/account/forgotPasswordDto.interface";
import {VerifyRequestDto} from "../interfaces/account/verifyEmailDto.interface";
import {ToastService} from "./toast.service";
import {Router} from "@angular/router";
import HttpService from "./http.service";
import {ChangePasswordDto} from "../interfaces/account/ChangePasswordDto";

@Injectable({
  providedIn: 'root'
})
export default class AccountService {
  constructor(private router: Router, private httpService: HttpService) {
  }

  public async setInitialLoginData(loginData: InitialLoginPostDto): Promise<void> {
    await this.httpService.put(environment.baseUrl + '/cycle/initial-login', loginData, 'Initial login data set');
  }

  public async checkIfUsernameIsTaken(username: string){
    return await this.httpService.get<boolean>(environment.baseUrl + '/identity/auth/username-taken/' + username);
  }

  public async isFirstLogin() {
    const response = await this.httpService.get<InitialLoginGetDto>(environment.baseUrl + '/cycle/initial-login');
    if(response === undefined || response === null){
      return false;
    }
    return response.periodLength === null || response.cycleLength === null;
  }

  public async verifyEmail(dto: VerifyRequestDto): Promise<void> {
    await this.httpService.post('/Identity/Account/Verify-Email', dto, 'Email verified');

    await this.router.navigateByUrl('/')
  }

  public async forgotPassword(dto: ForgotPasswordDto): Promise<void> {
    await this.httpService.post('/Identity/Account/Forgot-Password', dto, 'Reset password link sent to your email')

    await this.router.navigateByUrl('/')
  }

  public async resetPassword(dto: ResetPasswordDto): Promise<void> {
    await this.httpService.post('/Identity/Account/Reset-Password', dto, 'Your password was successfully reset');

    await this.router.navigateByUrl('/')
  }

  public async changePassword(dto: ChangePasswordDto): Promise<void> {
    await this.httpService.post('/Identity/Account/change-password', dto, 'Your password was successfully changed');
  }
}
