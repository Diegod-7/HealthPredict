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
  
  // ✅ USUARIOS PREDEFINIDOS PARA DESARROLLO
  usuariosPredefinidos = [
    { email: 'jefe@healthpredict.com', password: 'admin123', nombre: 'Carlos Rodriguez (Jefe)' },
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
        
        // Si es error 401, intentar inicializar datos primero
        if (error.status === 401) {
          console.log('🔄 Intentando inicializar datos de usuarios...');
          this.tryInitializeData(this.email, this.password);
        } else {
          this.errorMessage = 'Error de conexión. Verifica tu conexión a internet.';
          this.isLoading = false;
        }
      }
    });
  }

  /**
   * Maneja un login exitoso
   */
  private handleSuccessfulLogin(usuario: Usuario): void {
    // Calcular propiedades adicionales
    usuario.nombreCompleto = this.usuarioService.getNombreCompleto(usuario);
    usuario.esJefe = this.usuarioService.esJefe(usuario);
    usuario.esTrabajador = this.usuarioService.esTrabajador(usuario);
    
    // Guardar usuario en localStorage
    this.usuarioService.setCurrentUser(usuario);
    
    // Redirigir según el rol
    this.redirectToDashboard(usuario);
    
    this.isLoading = false;
  }

  /**
   * Intenta inicializar los datos de usuarios y luego hacer login
   */
  private tryInitializeData(email: string, password: string): void {
    this.usuarioService.inicializarDatos().subscribe({
      next: (response) => {
        console.log('✅ Datos inicializados:', response);
        
        // Intentar login nuevamente después de inicializar
        setTimeout(() => {
          this.usuarioService.authenticate(email, password).subscribe({
            next: (usuario) => {
              console.log('✅ Login exitoso después de inicializar:', usuario);
              this.handleSuccessfulLogin(usuario);
            },
            error: (retryError) => {
              console.error('❌ Error en segundo intento:', retryError);
              this.errorMessage = 'Credenciales incorrectas. Verifica tu email y contraseña.';
              this.isLoading = false;
            }
          });
        }, 1000); // Esperar 1 segundo para que se procesen los datos
      },
      error: (initError) => {
        console.error('❌ Error al inicializar datos:', initError);
        this.errorMessage = 'Credenciales incorrectas. Verifica tu email y contraseña.';
        this.isLoading = false;
      }
    });
  }

  /**
   * Login rápido con usuarios predefinidos
   */
  loginRapido(usuario: any): void {
    this.email = usuario.email;
    this.password = usuario.password;
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