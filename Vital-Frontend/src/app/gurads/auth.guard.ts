import {inject } from '@angular/core';
import { Router } from '@angular/router';
import {TokenService} from "../services/token.service";

export const authGuard = () => {
  const tokenService = inject(TokenService);
  const router = inject(Router);

  if (tokenService.isAuthenticated()) {
    return true;
  }

  // Redirect to the login page
  return router.parseUrl('/login');
};
