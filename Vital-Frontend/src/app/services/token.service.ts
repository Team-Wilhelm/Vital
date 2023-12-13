import {Injectable} from "@angular/core";
import {JwtHelperService} from "@auth0/angular-jwt";
import {environment} from "../../../environments/environment";
import {HttpClient} from "@angular/common/http";
import {firstValueFrom} from "rxjs";
import {LoginDto, RegisterDto} from "../interfaces/utilities.interface";
import HttpService from "./http.service";
import {ToastService} from "./toast.service";

@Injectable()
export class TokenService {
  private readonly storage: Storage = window.sessionStorage;

  constructor(public jwtHelper: JwtHelperService, private httpClient: HttpClient, private httpService: HttpService, private toastService: ToastService) {
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
        this.toastService.show('Invalid credentials', 'Error', 'error');
        throw new Error('Invalid credentials');
      }else if(e.status === 500){
        this.toastService.show('Something went wrong', 'Error', 'error');
        throw new Error('Something went wrong');
      }
    }
  }

  //TODO: try catch and user feedback
  public async register(registerDto: RegisterDto) {
    await this.httpService.post('/identity/auth/register', registerDto, 'Confirm your email')
  }

  public logout() {
    this.clearToken();
  }
}
