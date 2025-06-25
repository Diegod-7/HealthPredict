import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { AlertasListComponent } from './components/alertas-list/alertas-list.component';
import { AlertaDetalleComponent } from './components/alerta-detalle/alerta-detalle.component';
import { GraficosComponent } from './components/graficos/graficos.component';
import { LoginComponent } from './components/login/login.component';
import { DashboardJefeComponent } from './components/dashboard-jefe/dashboard-jefe.component';
import { AuthGuard } from './guards/auth.guard';

const routes: Routes = [
  // ✅ RUTA DE LOGIN (PÚBLICA)
  { path: 'login', component: LoginComponent },
  
  // ✅ RUTA RAÍZ - REDIRIGE SEGÚN AUTENTICACIÓN
  { path: '', redirectTo: '/login', pathMatch: 'full' },
  
  // ✅ RUTAS PROTEGIDAS PARA TRABAJADORES
  { 
    path: 'dashboard', 
    component: DashboardComponent,
    canActivate: [AuthGuard]
  },
  { 
    path: 'alertas', 
    component: AlertasListComponent,
    canActivate: [AuthGuard]
  },
  { 
    path: 'alertas/:id', 
    component: AlertaDetalleComponent,
    canActivate: [AuthGuard]
  },
  { 
    path: 'graficos', 
    component: GraficosComponent,
    canActivate: [AuthGuard]
  },
  {
    path: 'reportes',
    loadChildren: () => import('./components/reportes/reportes.module').then(m => m.ReportesModule),
    canActivate: [AuthGuard]
  },

  // ✅ RUTA EXCLUSIVA PARA JEFES
  { 
    path: 'dashboard-jefe', 
    component: DashboardJefeComponent,
    canActivate: [AuthGuard],
    data: { role: 'Jefe' }
  },

  // ✅ RUTA COMODÍN - REDIRIGE AL LOGIN
  { path: '**', redirectTo: '/login' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
