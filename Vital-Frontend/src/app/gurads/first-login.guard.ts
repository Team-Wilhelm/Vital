import {inject} from "@angular/core";
import {UserSessionService} from "../services/user-session.service";
import {Router} from "@angular/router";

export const firstLoginGuard = () => {
  const userSessionService = inject(UserSessionService);
  const router = inject(Router);

  // Allow access to dashboard, if the user has previously logged in and set the length of their cycle and period
  if (userSessionService.isAuthenticated() && !userSessionService.isFirstLogin()) {
    return true;
  }

  // Otherwise, redirect to a page, where the user can set the length of their cycle and period
  router.navigate(['/initial-login']);
  return false;
}
