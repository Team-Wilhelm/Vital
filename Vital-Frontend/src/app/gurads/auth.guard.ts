import {inject} from '@angular/core';
import {Router} from '@angular/router';
import {TokenService} from "../services/token.service";
import AccountService from "../services/account.service";

export const authGuard = async () => {
  const tokenService = inject(TokenService);
  const accountService = inject(AccountService);
  const router = inject(Router);

  // Allow access to login and register without authentication
  const allowedRoutes = ['/login', '/register'];
  if (allowedRoutes.includes(router.url)) {
    return true;
  }

  // If the user is authenticated, allow them to access the page
  if (tokenService.isAuthenticated()) {
    if (await accountService.isFirstLogin()) {
      return router.parseUrl('/initial-login');
    }
    return true;
  }

  // Redirect to the login page
  return router.parseUrl('/login');
};
