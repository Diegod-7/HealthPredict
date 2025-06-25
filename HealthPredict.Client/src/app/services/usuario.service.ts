import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Usuario } from '../models/usuario.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class UsuarioService {
  private apiUrl = `${environment.apiUrl}/usuarios`;

  constructor(private http: HttpClient) { }

  // ✅ MÉTODOS BÁSICOS USANDO DATOS REALES DEL SERVIDOR
  getUsuarios(): Observable<Usuario[]> {
    console.log('🌐 [REAL DATA] Obteniendo lista de usuarios del servidor');
    return this.http.get<Usuario[]>(this.apiUrl);
  }

  getUsuario(id: number): Observable<Usuario> {
    console.log('🌐 [REAL DATA] Obteniendo usuario ID:', id, 'del servidor');
    return this.http.get<Usuario>(`${this.apiUrl}/${id}`);
  }

  createUsuario(usuario: Usuario): Observable<Usuario> {
    console.log('🌐 [REAL DATA] Creando usuario en servidor:', usuario);
    return this.http.post<Usuario>(this.apiUrl, usuario);
  }

  updateUsuario(usuario: Usuario): Observable<void> {
    console.log('🌐 [REAL DATA] Actualizando usuario en servidor:', usuario);
    return this.http.put<void>(`${this.apiUrl}/${usuario.id}`, usuario);
  }

  deleteUsuario(id: number): Observable<void> {
    console.log('🌐 [REAL DATA] Eliminando usuario ID:', id, 'del servidor');
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  authenticate(email: string, password: string): Observable<Usuario> {
    console.log('🌐 [REAL DATA] Autenticando usuario:', email, 'en servidor');
    return this.http.post<Usuario>(`${this.apiUrl}/authenticate`, { email, password });
  }

  // ✅ NUEVOS MÉTODOS PARA SISTEMA DE PERFILAMIENTO

  /**
   * Obtiene todos los jefes del sistema
   */
  getJefes(): Observable<Usuario[]> {
    console.log('🌐 [REAL DATA] Obteniendo jefes del servidor');
    return this.http.get<Usuario[]>(`${this.apiUrl}/Jefes`);
  }

  /**
   * Obtiene todos los trabajadores del sistema
   */
  getTrabajadores(): Observable<Usuario[]> {
    console.log('🌐 [REAL DATA] Obteniendo trabajadores del servidor');
    return this.http.get<Usuario[]>(`${this.apiUrl}/Trabajadores`);
  }

  /**
   * Obtiene los subordinados de un jefe específico
   */
  getSubordinadosByJefe(jefeId: number): Observable<Usuario[]> {
    console.log('🌐 [REAL DATA] Obteniendo subordinados del jefe ID:', jefeId);
    return this.http.get<Usuario[]>(`${this.apiUrl}/Jefe/${jefeId}/Subordinados`);
  }

  /**
   * Obtiene estadísticas del dashboard para un jefe
   */
  getDashboardJefe(jefeId: number): Observable<any> {
    console.log('🌐 [REAL DATA] Obteniendo dashboard del jefe ID:', jefeId);
    return this.http.get<any>(`${this.apiUrl}/Dashboard/Jefe/${jefeId}`);
  }

  /**
   * Obtiene usuarios por departamento
   */
  getUsuariosByDepartamento(departamento: string): Observable<Usuario[]> {
    console.log('🌐 [REAL DATA] Obteniendo usuarios del departamento:', departamento);
    return this.http.get<Usuario[]>(`${this.apiUrl}/Departamento/${departamento}`);
  }

  /**
   * Verifica si un usuario puede acceder a los datos de otro
   */
  verificarAcceso(usuarioSolicitante: number, usuarioObjetivo: number): Observable<boolean> {
    return this.http.get<boolean>(`${this.apiUrl}/VerificarAcceso/${usuarioSolicitante}/${usuarioObjetivo}`);
  }

  // ✅ MÉTODOS AUXILIARES PARA EL FRONTEND

  /**
   * Calcula el nombre completo de un usuario
   */
  getNombreCompleto(usuario: Usuario): string {
    return `${usuario.nombre} ${usuario.apellido}`;
  }

  /**
   * Verifica si un usuario es jefe
   */
  esJefe(usuario: Usuario): boolean {
    return usuario.rol === 'Jefe';
  }

  /**
   * Verifica si un usuario es trabajador
   */
  esTrabajador(usuario: Usuario): boolean {
    return usuario.rol === 'Trabajador';
  }

  /**
   * Obtiene el usuario actual desde localStorage (para persistencia)
   */
  getCurrentUser(): Usuario | null {
    const userData = localStorage.getItem('currentUser');
    return userData ? JSON.parse(userData) : null;
  }

  /**
   * Guarda el usuario actual en localStorage
   */
  setCurrentUser(usuario: Usuario): void {
    localStorage.setItem('currentUser', JSON.stringify(usuario));
  }

  /**
   * Elimina el usuario actual del localStorage
   */
  logout(): void {
    localStorage.removeItem('currentUser');
  }

  /**
   * Inicializa los datos de usuarios si no existen (método temporal)
   */
  inicializarDatos(): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/inicializar-datos`, {});
  }
}
