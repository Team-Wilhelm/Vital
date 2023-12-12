import {ActivatedRouteSnapshot, CanActivateFn, Router, RouterStateSnapshot} from '@angular/router';
import {inject} from "@angular/core";
import AccountService from "../services/account.service";


export const emailLinkGuard: CanActivateFn =
  async (route: ActivatedRouteSnapshot, state: RouterStateSnapshot) => {

    const userId = route.queryParams["userId"]!;
    const token = route.queryParams["token"]!;
    const verifyRequestDto = {userId, token};
    const accountService = inject(AccountService);
    const router = inject(Router);

    // Route to login page if the token is invalid
    if(!userId || !token || !await accountService.isValidTokenForUser(verifyRequestDto)) {
      return router.parseUrl('/login');
    }

    return true;

  };
