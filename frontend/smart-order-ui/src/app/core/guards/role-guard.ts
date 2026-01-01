import { CanActivateFn, CanActivateChildFn, Router } from '@angular/router'
import { inject } from '@angular/core'
import { AuthService } from '../services/auth-service'

const checkRole = (route: any) => {
  const auth = inject(AuthService)
  const router = inject(Router)

  const allowedRoles = route.data?.['roles'] as string[]
  const userRole = auth.getRole()

  if (!allowedRoles || !allowedRoles.includes(userRole)) {
    router.navigate(['/login'])
    return false
  }
  return true
}

export const roleGuard: CanActivateFn = route => checkRole(route)
export const roleChildGuard: CanActivateChildFn = route => checkRole(route)
