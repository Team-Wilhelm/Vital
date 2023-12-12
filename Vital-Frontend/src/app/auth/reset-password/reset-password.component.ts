import {Component, OnInit} from "@angular/core";
import {ResetPasswordDto} from "../../interfaces/account/resetPasswordDto.interface";
import AccountService from "../../services/account.service";
import {ActivatedRoute} from "@angular/router";

@Component({
  selector: 'reset-password',
  templateUrl: './reset-password.component.html'

})
export class ResetPasswordComponent implements OnInit{
  dto: ResetPasswordDto = { userId: '', token: '', newPassword: '' }; // Declare the model

  constructor(private accountService: AccountService, private route: ActivatedRoute) { } // Inject the service

  ngOnInit(): void {
    // Get userId and token from query parameters and update the dto
    this.route.queryParams.subscribe(params => {
      this.dto.userId = params['userId'];
      this.dto.token = decodeURIComponent(params['token']);
    });
  }

  resetPassword(): void {
    if (this.dto.userId && this.dto.token && this.dto.newPassword) {
      this.accountService.resetPassword(this.dto); // Call the method from the AccountService
    }
  }
}
