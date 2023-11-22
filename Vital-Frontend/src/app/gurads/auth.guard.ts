import {inject } from '@angular/core';
import { Router } from '@angular/router';
import {TokenService} from "../services/token.service";

export const authGuard = () => {
  const tokenService = inject(TokenService);
  const router = inject(Router);

  // Allow access to login and register without authentication
  const allowedRoutes = ['/login', '/register'];
  if (allowedRoutes.includes(router.url)) {
    return true;
  }

  // If the user is authenticated, allow them to access the page
  if (tokenService.isAuthenticated()) {
    return true;
  }

  // Redirect to the login page
  console.log('User is not authenticated, redirecting to login page');
  return router.parseUrl('/login');
};
