import {inject} from "@angular/core";
import {TokenService} from "../services/token.service";
import AccountService from "../services/account.service";
import {Router} from "@angular/router";

export const initialLoginGuard = async () => {
  const tokenService = inject(TokenService);
  const accountService = inject(AccountService);
  const router = inject(Router);

  if (tokenService.isAuthenticated()) {
    if (await accountService.isFirstLogin()) {
      return true;
    }
    return router.parseUrl('/dashboard');
  }

  // Redirect to the login page
  return router.parseUrl('/login');
};
