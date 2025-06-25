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

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
    const currentUser = this.usuarioService.getCurrentUser();
    
    if (currentUser) {
      // Verificar si es Carlos Rodríguez y promocionarlo temporalmente a jefe
      if (currentUser.email === 'carlos.rodriguez@example.com' && currentUser.rol !== 'Jefe') {
        console.log('👔 Promoviendo a Carlos Rodríguez como jefe en guard');
        currentUser.rol = 'Jefe';
        currentUser.esJefe = true;
        currentUser.esTrabajador = false;
        this.usuarioService.setCurrentUser(currentUser);
      }

      // Verificar permisos para rutas específicas
      const url = state.url;
      
      if (url.includes('/dashboard-jefe')) {
        if (!currentUser.esJefe && currentUser.email !== 'carlos.rodriguez@example.com') {
          console.log('❌ Acceso denegado a dashboard jefe');
          this.router.navigate(['/dashboard']);
          return false;
        }
      }
      
      return true;
    } else {
      console.log('❌ Usuario no autenticado, redirigiendo a login');
      this.router.navigate(['/login']);
      return false;
    }
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