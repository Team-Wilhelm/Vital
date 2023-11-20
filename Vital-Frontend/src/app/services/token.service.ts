import {Injectable} from "@angular/core";
import {JwtHelperService} from "@auth0/angular-jwt";
import {environment} from "../../../environments/environment";
import {HttpClient} from "@angular/common/http";
import {firstValueFrom} from "rxjs";
import {LoginDto} from "../interfaces/Utilities";

@Injectable()
export class TokenService {
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
    const request = this.httpClient.post<any>(environment.baseUrl + '/identity/auth/login', loginDto);
    const response = await firstValueFrom(request);
    const token = response.token;
    this.setToken(token);
  }
}
