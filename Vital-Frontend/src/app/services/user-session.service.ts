import {Injectable} from "@angular/core";
import {JwtHelperService} from "@auth0/angular-jwt";
import {environment} from "../../../environments/environment";
import {HttpClient} from "@angular/common/http";
import {firstValueFrom} from "rxjs";
import {LoginDto, RegisterDto} from "../interfaces/Utilities";
import {ApplicationUserInitialLoginDto} from "../interfaces/dtos/user.dto.interface";

@Injectable()
export class UserSessionService {
  private readonly storage: Storage = window.sessionStorage;

  constructor(public jwtHelper: JwtHelperService, private httpClient: HttpClient) {
  }

  setToken(token: string) {
    this.storage.setItem("token", token);
  }

  getToken() {
    return this.storage.getItem("token");
  }

  clearToken() {
    this.storage.removeItem("token");
  }

  public isAuthenticated(): boolean {
    const token = this.getToken();
    return !this.jwtHelper.isTokenExpired(token);
  }

  public async login(loginDto: LoginDto) {
    try {
      const request = this.httpClient.post<any>(environment.baseUrl + '/identity/auth/login', loginDto);
      const response = await firstValueFrom(request);
      const token = response.token;
      this.setToken(token);
    } catch (e : any) {
      if (e.status === 400) {
        throw new Error('Invalid credentials');
      }
    }
  }

  public async register(registerDto: RegisterDto) {
    const request = this.httpClient.post<any>(environment.baseUrl + '/identity/auth/register', registerDto);
    await firstValueFrom(request);
  }

  public logout() {
    this.clearToken();
  }

  public async isFirstLogin(): Promise<boolean> {
    const request = this.httpClient.get<ApplicationUserInitialLoginDto>(environment.baseUrl + '/identity/account/initial-login');
    const response = await firstValueFrom(request);
    return response.periodLength === null || response.cycleLength === null;
  }

  public async setInitialLoginData(periodLength: number, cycleLength: number): Promise<void> {
    const request = this.httpClient.put(environment.baseUrl + '/identity/account/initial-login', {
      periodLength: periodLength,
      cycleLength: cycleLength
    });
    await firstValueFrom(request);
  }

  public async checkIfUsernameIsTaken(username: string): Promise<boolean> {
    const request = this.httpClient.get<boolean>(environment.baseUrl + '/identity/auth/username-taken/' + username);
    return await firstValueFrom(request);
  }
}
