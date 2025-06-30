import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { UsuarioService } from '../../services/usuario.service';
import { Usuario } from '../../models/usuario.model';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {
  
  // ✅ CREDENCIALES DE LOGIN
  email: string = '';
  password: string = '';
  
  // ✅ ESTADO DEL COMPONENTE
  isLoading: boolean = false;
  errorMessage: string = '';
  
  // ✅ USUARIOS PREDEFINIDOS PARA DESARROLLO - CREDENCIALES CORRECTAS
  usuariosPredefinidos = [
    { email: 'carlos.rodriguez@healthpredict.com', password: 'admin123', nombre: 'Carlos Rodriguez (Jefe)' },
    { email: 'diego.diaz@healthpredict.com', password: 'diego123', nombre: 'Diego Diaz (Trabajador)' },
    { email: 'matias.maripangue@healthpredict.com', password: 'matias123', nombre: 'Matias Maripangue (Trabajador)' },
    { email: 'iahn.vera@healthpredict.com', password: 'iahn123', nombre: 'Iahn Vera (Trabajador)' }
  ];

  constructor(
    private usuarioService: UsuarioService,
    private router: Router
  ) { }

  ngOnInit(): void {
    // Verificar si ya hay un usuario logueado
    const currentUser = this.usuarioService.getCurrentUser();
    if (currentUser) {
      this.redirectToDashboard(currentUser);
    }
  }

  /**
   * Realiza el login del usuario
   */
  onLogin(): void {
    if (!this.email || !this.password) {
      this.errorMessage = 'Por favor, ingresa email y contraseña';
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';
    
    console.log('🔐 Intentando login para:', this.email);

    this.usuarioService.authenticate(this.email, this.password).subscribe({
      next: (usuario) => {
        console.log('✅ Login exitoso:', usuario);
        this.handleSuccessfulLogin(usuario);
      },
      error: (error) => {
        console.error('❌ Error en login:', error);
        
        if (error.status === 401) {
          this.errorMessage = 'Credenciales incorrectas. Por favor verifica tu email y contraseña.';
        } else if (error.status === 0) {
          this.errorMessage = 'Error de conexión. Verifica tu conexión a internet.';
        } else {
          this.errorMessage = 'Error del servidor. Inténtalo más tarde.';
        }
        
        this.isLoading = false;
      }
    });
  }

  /**
   * Maneja un login exitoso
   */
  private handleSuccessfulLogin(response: any): void {
    console.log('🔐 Respuesta recibida del backend:', response);
    
    // Extraer el usuario de la respuesta
    let usuario: Usuario;
    if (response && response.usuario) {
      // Caso: { success: true, usuario: {...} }
      usuario = response.usuario;
    } else if (response && response.email) {
      // Caso: directamente el objeto usuario
      usuario = response;
    } else {
      console.error('❌ Respuesta inválida del backend:', response);
      this.errorMessage = 'Error: Datos de usuario inválidos';
      this.isLoading = false;
      return;
    }
    
    // Validar que el usuario tenga información básica
    if (!usuario || !usuario.email) {
      console.error('❌ Usuario inválido extraído de la respuesta');
      this.errorMessage = 'Error: Datos de usuario inválidos';
      this.isLoading = false;
      return;
    }
    
    // Asegurar que las propiedades básicas existan
    if (!usuario.nombre) usuario.nombre = '';
    if (!usuario.apellido) usuario.apellido = '';
    
    // Transformar usuarios existentes a los nombres que queremos (para compatibilidad con datos legacy)
    if (usuario.email === 'juan.perez@example.com') {
      usuario.nombre = 'Diego';
      usuario.apellido = 'Diaz';
    } else if (usuario.email === 'maria.gonzalez@example.com') {
      usuario.nombre = 'Iahn';
      usuario.apellido = 'Vera';
    } else if (usuario.email === 'carlos.rodriguez@example.com') {
      usuario.nombre = 'Carlos';
      usuario.apellido = 'Rodríguez';
      usuario.rol = 'Jefe'; // Asegurar que Carlos sea jefe
    }
    
    // Calcular nombreCompleto si no existe
    if (!usuario.nombreCompleto || usuario.nombreCompleto.trim() === '') {
      usuario.nombreCompleto = this.usuarioService.getNombreCompleto(usuario);
    }
    
    // Si nombreCompleto sigue vacío, usar el email como fallback
    if (!usuario.nombreCompleto || usuario.nombreCompleto.trim() === '' || usuario.nombreCompleto === ' ') {
      if (usuario.email && typeof usuario.email === 'string') {
        const emailName = usuario.email.split('@')[0];
        usuario.nombreCompleto = emailName.charAt(0).toUpperCase() + emailName.slice(1);
      } else {
        usuario.nombreCompleto = 'Usuario';
      }
    }
    
    // Calcular propiedades adicionales
    usuario.esJefe = this.usuarioService.esJefe(usuario);
    usuario.esTrabajador = this.usuarioService.esTrabajador(usuario);
    
    // Si es Carlos Rodríguez por email, forzar que sea jefe
    if (usuario.email === 'carlos.rodriguez@healthpredict.com' || usuario.email === 'carlos.rodriguez@example.com') {
      usuario.rol = 'Jefe';
      usuario.esJefe = true;
      usuario.esTrabajador = false;
      if (!usuario.nombreCompleto || usuario.nombreCompleto.trim() === '') {
        usuario.nombreCompleto = 'Carlos Rodríguez';
      }
    }
    
    console.log('✅ Usuario procesado para guardar:', usuario);
    console.log('📧 Email:', usuario.email);
    console.log('👤 Nombre completo:', usuario.nombreCompleto);
    console.log('👔 Rol:', usuario.rol);
    console.log('🎯 Es jefe:', usuario.esJefe);
    
    // Guardar usuario en localStorage
    this.usuarioService.setCurrentUser(usuario);
    
    // Redirigir según el rol
    this.redirectToDashboard(usuario);
    
    this.isLoading = false;
  }

  /**
   * Login rápido con credenciales predefinidas
   */
  loginRapido(email: string, password: string): void {
    if (this.isLoading) return;
    
    this.email = email;
    this.password = password;
    this.onLogin();
  }

  /**
   * Redirige al dashboard apropiado según el rol del usuario
   */
  private redirectToDashboard(usuario: Usuario): void {
    if (this.usuarioService.esJefe(usuario)) {
      console.log('🚀 Redirigiendo a dashboard de jefe');
      this.router.navigate(['/dashboard-jefe']);
    } else {
      console.log('🚀 Redirigiendo a dashboard de trabajador');
      this.router.navigate(['/dashboard']);
    }
  }

  /**
   * Limpia los campos del formulario
   */
  limpiarFormulario(): void {
    this.email = '';
    this.password = '';
    this.errorMessage = '';
  }
} 