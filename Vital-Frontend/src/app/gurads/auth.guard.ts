import {inject } from '@angular/core';
import { Router } from '@angular/router';
import {TokenService} from "../services/token.service";

export const authGuard = () => {
  const tokenService = inject(TokenService);
  const router = inject(Router);

  // If the user is authenticated, allow them to access the page
  if (tokenService.isAuthenticated()) {
    return true;
  }

  // Redirect to the login page
  console.log('User is not authenticated, redirecting to login page');
  return router.parseUrl('/login');
};
