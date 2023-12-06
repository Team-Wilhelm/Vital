import {Component} from "@angular/core";
import {ResetPasswordDto} from "../../interfaces/account/resetPasswordDto.interface";
import AccountService from "../../services/account.service";

@Component({
  selector: 'reset-password',
  templateUrl: './reset-password.component.html'

})
export class ResetPasswordComponent {
  dto: ResetPasswordDto = { userId: '', token: '', newPassword: '' }; // Declare the model

  constructor(private accountService: AccountService) { } // Inject the service

  resetPassword(): void {
    if (this.dto.userId && this.dto.token && this.dto.newPassword) {
      this.accountService.resetPassword(this.dto); // Call the method from the AccountService
    }
  }
}
