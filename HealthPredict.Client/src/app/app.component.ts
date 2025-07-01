import { Component, OnInit } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';
import { UsuarioService } from './services/usuario.service';
import { Usuario } from './models/usuario.model';
import { filter } from 'rxjs/operators';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  title = 'HealthPredict.Client';
  isAuthenticated: boolean = false;
  currentUser: Usuario | null = null;

  constructor(
    private usuarioService: UsuarioService,
    private router: Router
  ) {}

  ngOnInit(): void {
    // Verificar autenticación al inicializar
    this.checkAuthentication();

    // Escuchar cambios de ruta para verificar autenticación
    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd)
    ).subscribe(() => {
      this.checkAuthentication();
    });
  }

  /**
   * Verifica si el usuario está autenticado
   */
  private checkAuthentication(): void {
    this.currentUser = this.usuarioService.getCurrentUser();
    this.isAuthenticated = this.currentUser !== null;
  }

  /**
   * Obtiene el nombre a mostrar del usuario con fallback
   */
  getNombreParaMostrar(usuario: Usuario): string {
    if (usuario.nombreCompleto && usuario.nombreCompleto.trim() !== '') {
      return usuario.nombreCompleto;
    }
    
    if (usuario.nombre && usuario.apellido) {
      return `${usuario.nombre} ${usuario.apellido}`;
    }
    
    if (usuario.nombre) {
      return usuario.nombre;
    }
    
    // Fallback al email si no hay nombre
    if (usuario.email && typeof usuario.email === 'string') {
      const emailName = usuario.email.split('@')[0];
      return emailName.charAt(0).toUpperCase() + emailName.slice(1);
    }
    
    return 'Usuario';
  }

  /**
   * Verifica si el usuario actual es jefe
   */
  isJefe(): boolean {
    if (!this.currentUser) return false;
    return this.currentUser.rol === 'Jefe' || 
           this.currentUser.esJefe === true ||
           this.currentUser.email === 'carlos.rodriguez@healthpredict.com' ||
           this.currentUser.email === 'carlos.rodriguez@example.com';
  }

  /**
   * Cierra la sesión del usuario
   */
  logout(): void {
    this.usuarioService.logout();
    this.isAuthenticated = false;
    this.currentUser = null;
    this.router.navigate(['/login']);
  }
}
