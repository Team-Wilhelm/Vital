import {Injectable} from "@angular/core";
import {HttpClient} from "@angular/common/http";
import {firstValueFrom} from "rxjs";
import {environment} from "../../../environments/environment";

@Injectable({
  providedIn: 'root'
})

export class UserService{
  constructor(private httpClient: HttpClient) {
  }

  async getUserEmail() {
    const userEmail = await firstValueFrom(this.httpClient.get<UserEmailDto>(environment.baseUrl + '/identity/account/email'));
    return userEmail.email;
  }
}

interface UserEmailDto {
  email: string;
}
