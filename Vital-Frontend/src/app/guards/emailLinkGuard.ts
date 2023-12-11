import { ActivatedRouteSnapshot, Router } from '@angular/router';
import {inject} from '@angular/core';
import AccountService from "../services/account.service";


export const emailLinkGuard = async () => {
  const router = inject(Router);
  const route = inject(ActivatedRouteSnapshot);
  const accountService = inject(AccountService);

  const userId = route.queryParams['userId'];
  const token = route.queryParams['token'];
  const verifyRequestDto = { userId: userId, token: token };

  if (route.queryParams['userId'] && route.queryParams['token']) {
    if (route.url[0].path === 'verify-email' && await accountService.isValidTokenForUser(verifyRequestDto)) {
      router.parseUrl('/verify-email');
      return true;
    }
    if (route.url[0].path === 'reset-password' && await accountService.isValidTokenForUser(verifyRequestDto)) {
      router.parseUrl('/reset-password');
      return true;
    }
  }

  // Redirect to the login page
  return router.parseUrl('/login');
};
