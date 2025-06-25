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
    console.log('🔧 Datos no encontrados. Verificando credenciales localmente...');
    
    // Credenciales válidas temporales hasta que se inicialicen los datos en el servidor
    const credencialesValidas = [
      { email: 'jefe@healthpredict.com', password: 'admin123', rol: 'Jefe', nombre: 'Carlos', apellido: 'Rodriguez' },
      { email: 'diego.diaz@healthpredict.com', password: 'diego123', rol: 'Trabajador', nombre: 'Diego', apellido: 'Diaz' },
      { email: 'matias.maripangue@healthpredict.com', password: 'matias123', rol: 'Trabajador', nombre: 'Matias', apellido: 'Maripangue' },
      { email: 'iahn.vera@healthpredict.com', password: 'iahn123', rol: 'Trabajador', nombre: 'Iahn', apellido: 'Vera' }
    ];

    const credencial = credencialesValidas.find(c => c.email === email && c.password === password);
    
    if (credencial) {
      console.log('✅ Credenciales válidas encontradas localmente');
      
      // Crear usuario temporal
      const usuarioTemporal: Usuario = {
        id: credencial.email === 'jefe@healthpredict.com' ? 1 : 
            credencial.email === 'diego.diaz@healthpredict.com' ? 2 :
            credencial.email === 'matias.maripangue@healthpredict.com' ? 3 : 4,
        nombre: credencial.nombre,
        apellido: credencial.apellido,
        email: credencial.email,
        password: credencial.password,
        fechaNacimiento: new Date(1990, 1, 1),
        genero: 'Masculino',
        altura: 175,
        peso: 70,
        fechaRegistro: new Date(),
        ultimoAcceso: new Date(),
        esProfesionalMedico: false,
        rol: credencial.rol,
        departamento: credencial.rol === 'Jefe' ? 'Administración' : 'Desarrollo',
        cargo: credencial.rol === 'Jefe' ? 'Gerente General' : 'Desarrollador',
        jefeId: credencial.rol === 'Trabajador' ? 1 : undefined,
        esActivo: true
      };

      this.handleSuccessfulLogin(usuarioTemporal);
    } else {
      console.log('❌ Credenciales no válidas');
      this.errorMessage = 'Credenciales incorrectas. Usa uno de los usuarios de prueba.';
      this.isLoading = false;
    }
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