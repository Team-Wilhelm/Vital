import {Component} from "@angular/core";
import AccountService from "../../services/account.service";
import {VerifyRequestDto} from "../../interfaces/account/verifyEmailDto.interface";

@Component({
  selector: 'verify-email',
  templateUrl: './verify-email.component.html'

})
export class VerifyEmailComponent {
  dto: VerifyRequestDto = { userId: '', token: '' }; // Declare the model

  constructor(private accountService: AccountService) { } // Inject the service

  verifyEmail(): void {
    if (this.dto.userId && this.dto.token) {
      this.accountService.verifyEmail(this.dto); // Call the method from the AccountService
    }
  }
}
