import { Injectable } from '@angular/core';
import { CanActivate, Router, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { Observable } from 'rxjs';
import { UsuarioService } from '../services/usuario.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {

  constructor(
    private usuarioService: UsuarioService,
    private router: Router
  ) {}

  canActivate(
    next: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<boolean> | Promise<boolean> | boolean {
    
    const currentUser = this.usuarioService.getCurrentUser();
    
    if (!currentUser) {
      // Usuario no autenticado, redirigir al login
      console.log('🔒 Acceso denegado: Usuario no autenticado');
      this.router.navigate(['/login']);
      return false;
    }

    const requiredRole = next.data['role'];
    const currentPath = state.url;

    // Si es la ruta de login y ya está autenticado, redirigir según su rol
    if (currentPath === '/login') {
      this.redirectUserToCorrectDashboard(currentUser.rol);
      return false;
    }

    // Si la ruta requiere un rol específico
    if (requiredRole) {
      if (currentUser.rol === requiredRole) {
        console.log(`✅ Acceso permitido: Usuario con rol ${currentUser.rol} accediendo a ruta para ${requiredRole}`);
        return true;
      } else {
        console.log(`❌ Acceso denegado: Usuario con rol ${currentUser.rol} intentando acceder a ruta para ${requiredRole}`);
        this.redirectUserToCorrectDashboard(currentUser.rol);
        return false;
      }
    }

    // Si un jefe intenta acceder a rutas de trabajador, redirigir a su dashboard
    if (currentUser.rol === 'Jefe' && !requiredRole) {
      if (currentPath !== '/dashboard-jefe') {
        console.log('🔄 Jefe redirigido a su dashboard específico');
        this.router.navigate(['/dashboard-jefe']);
        return false;
      }
    }

    // Si un trabajador intenta acceder al dashboard del jefe
    if (currentUser.rol === 'Trabajador' && currentPath === '/dashboard-jefe') {
      console.log('🚫 Trabajador intentando acceder a dashboard de jefe');
      this.router.navigate(['/dashboard']);
      return false;
    }

    console.log(`✅ Acceso permitido: ${currentUser.rol} accediendo a ${currentPath}`);
    return true;
  }

  /**
   * Redirige al usuario a su dashboard correspondiente según su rol
   */
  private redirectUserToCorrectDashboard(rol: string): void {
    if (rol === 'Jefe') {
      this.router.navigate(['/dashboard-jefe']);
    } else {
      this.router.navigate(['/dashboard']);
    }
  }
} 