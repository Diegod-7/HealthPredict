import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { AlertasListComponent } from './components/alertas-list/alertas-list.component';
import { AlertaDetalleComponent } from './components/alerta-detalle/alerta-detalle.component';
import { GraficosComponent } from './components/graficos/graficos.component';

const routes: Routes = [
  { path: '', redirectTo: '/dashboard', pathMatch: 'full' },
  { path: 'dashboard', component: DashboardComponent },
  { path: 'alertas', component: AlertasListComponent },
  { path: 'alertas/:id', component: AlertaDetalleComponent },
  { path: 'graficos', component: GraficosComponent },
  { path: '**', redirectTo: '/dashboard' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
