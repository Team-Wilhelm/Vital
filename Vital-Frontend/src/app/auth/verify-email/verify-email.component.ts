import {Component, OnInit} from "@angular/core";
import AccountService from "../../services/account.service";
import {VerifyRequestDto} from "../../interfaces/account/verifyEmailDto.interface";
import {ActivatedRoute} from "@angular/router";

@Component({
  selector: 'verify-email',
  templateUrl: './verify-email.component.html'

})
export class VerifyEmailComponent implements OnInit{
  dto: VerifyRequestDto = { userId: '', token: '' }; // Declare the model

  constructor(
    private accountService: AccountService,  // Inject the service
    private route: ActivatedRoute  // Inject ActivatedRoute
  ) { }

  ngOnInit(): void {
    // Get userId and token from query parameters and update the dto
    this.route.queryParams.subscribe(params => {
      this.dto.userId = params['userId'];
      this.dto.token = decodeURIComponent(params['token']);
    });
  }

  verifyEmail(): void {
    if (this.dto.userId && this.dto.token) {
      this.accountService.verifyEmail(this.dto); // Call the method from the AccountService
    }
  }
}
