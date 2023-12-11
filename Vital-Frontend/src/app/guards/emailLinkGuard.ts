import {ActivatedRoute, ActivatedRouteSnapshot, Router} from '@angular/router';
import {inject} from '@angular/core';
import AccountService from "../services/account.service";
import {firstValueFrom} from "rxjs";


export const emailLinkGuard = async () => {
  return new Promise(async (resolve, reject) => {
    const router = inject(Router);
    const activatedRoute = inject(ActivatedRoute);
    const route: ActivatedRouteSnapshot = activatedRoute.snapshot;
    const accountService = inject(AccountService);

    const params = await firstValueFrom(activatedRoute.queryParams);
    console.log(params);

    activatedRoute.queryParams.subscribe(async params => {
      console.log(params);
      const userId = params['userId'];
      const token = decodeURI(params['token']);
      const verifyRequestDto = {userId: userId, token: token};
      console.log(verifyRequestDto);

     if (userId && token) {
        const isTokenValid = await accountService.isValidTokenForUser(verifyRequestDto);
        if (route.url[0].path === 'verify-email' && isTokenValid) {
          router.parseUrl('/verify-email');
          resolve(true);
        }
        else if (route.url[0].path === 'reset-password' && isTokenValid) {
          router.parseUrl('/reset-password');
          resolve(true);
        } else {
          resolve(router.parseUrl('/login'));
        }
      }

      // Redirect to the login page
      resolve(router.parseUrl('/login'));
    });
  });
}
