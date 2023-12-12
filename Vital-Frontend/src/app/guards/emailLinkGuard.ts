import {ActivatedRouteSnapshot, CanActivateFn, Router, RouterStateSnapshot} from '@angular/router';
import {inject} from "@angular/core";
import AccountService from "../services/account.service";


export const emailLinkGuard: CanActivateFn =
  async (route: ActivatedRouteSnapshot, state: RouterStateSnapshot) => {

    const userId = route.queryParams["userId"]!;
    const token = route.queryParams["token"]!;
    const url = state.url.split('?')[0];
    const verifyRequestDto = {userId, token};
    const accountService = inject(AccountService);
    const router = inject(Router);

    if(userId && token && await accountService.isValidTokenForUser(verifyRequestDto)) {
        return router.parseUrl(url);
    }

    // Redirect to the login page
    return router.parseUrl('/login');
  };
