import { inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { CanActivateFn, Router } from '@angular/router';
import { Cutomerservice } from '../customer/service/cutomerservice';

export const adminGuard: CanActivateFn = () => {
  const platformId = inject(PLATFORM_ID);
  if (!isPlatformBrowser(platformId)) {
    return true;
  }

  const customerService = inject(Cutomerservice);
  const router = inject(Router);

  if (customerService.isAdmin()) {
    return true;
  }

  return router.createUrlTree([customerService.isAuthenticated() ? '/' : '/login']);
};