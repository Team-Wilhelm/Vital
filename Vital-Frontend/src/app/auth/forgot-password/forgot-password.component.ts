import {Component} from "@angular/core";
import {ForgotPasswordDto} from "../../interfaces/account/forgotPasswordDto.interface";
import AccountService from "../../services/account.service";

@Component({
  selector: 'forgot-password',
  templateUrl: './forgot-password.component.html'

})
export class ForgotPasswordComponent {
  dto: ForgotPasswordDto = { email: '' };

  constructor(private accountService: AccountService) { }

  forgotPassword(): void {
    if (this.dto.email) {
      this.accountService.forgotPassword(this.dto); // Call the method from the AccountService
    }
  }
}
