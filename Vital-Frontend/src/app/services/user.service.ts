import {Injectable} from "@angular/core";
import {HttpClient} from "@angular/common/http";
import {firstValueFrom} from "rxjs";
import {environment} from "../../../environments/environment";
import HttpService from "./http.service";

@Injectable({
  providedIn: 'root'
})

export class UserService{
  constructor(private httpService: HttpService) {
  }

  async getUserEmail() {
    const userEmail = await this.httpService.get<UserEmailDto>('/identity/account/email') ?? {} as UserEmailDto;
    return userEmail.email;
  }
}

interface UserEmailDto {
  email: string;
}
