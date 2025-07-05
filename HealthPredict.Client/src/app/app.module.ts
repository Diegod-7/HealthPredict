import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ChartsModule } from '@rinminase/ng-charts';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

// Angular Material
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatDialogModule } from '@angular/material/dialog';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { AlertasListComponent } from './components/alertas-list/alertas-list.component';
import { AlertaDetalleComponent } from './components/alerta-detalle/alerta-detalle.component';
import { GraficosComponent } from './components/graficos/graficos.component';
import { LineaTemporalComponent } from './components/graficos/linea-temporal/linea-temporal.component';
import { ResumenEstadisticasComponent } from './components/graficos/resumen-estadisticas/resumen-estadisticas.component';
import { ComparativaMensualComponent } from './components/graficos/comparativa-mensual/comparativa-mensual.component';

// ✅ NUEVOS COMPONENTES DEL SISTEMA DE PERFILAMIENTO
import { LoginComponent } from './components/login/login.component';
import { DashboardJefeComponent } from './components/dashboard-jefe/dashboard-jefe.component';
import { InteligenciaPredictivaComponent } from './components/inteligencia-predictiva/inteligencia-predictiva.component';
import { AnalyticsEmpresarialComponent } from './components/analytics-empresarial/analytics-empresarial.component';

// Módulo de reportes
import { ReportesModule } from './components/reportes/reportes.module';

// ✅ COMPONENTE DE SENSORES DE SALUD
import { HealthSensorComponent } from './components/health-sensor/health-sensor.component';

// ✅ COMPONENTE DE SINCRONIZACIÓN CON GOOGLE DRIVE
import { GoogleDriveSyncComponent } from './components/google-drive-sync/google-drive-sync.component';

// ✅ COMPONENTE DE GRÁFICO DE PASOS
import { PasosChartComponent } from './components/pasos-chart/pasos-chart.component';

@NgModule({
  declarations: [
    AppComponent,
    DashboardComponent,
    AlertasListComponent,
    AlertaDetalleComponent,
    GraficosComponent,
    LineaTemporalComponent,
    ResumenEstadisticasComponent,
    ComparativaMensualComponent,
    // ✅ COMPONENTES DEL SISTEMA DE PERFILAMIENTO
    LoginComponent,
    DashboardJefeComponent,
    InteligenciaPredictivaComponent,
    AnalyticsEmpresarialComponent,
    // ✅ COMPONENTE DE SENSORES DE SALUD
    HealthSensorComponent,
    // ✅ COMPONENTE DE SINCRONIZACIÓN CON GOOGLE DRIVE
    GoogleDriveSyncComponent,
    // ✅ COMPONENTE DE GRÁFICO DE PASOS
    PasosChartComponent
  ],
  imports: [
    BrowserModule,
    CommonModule,
    AppRoutingModule,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    ChartsModule,
    ReportesModule,
    BrowserAnimationsModule,
    // Angular Material
    MatButtonModule,
    MatCardModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatSnackBarModule,
    MatFormFieldModule,
    MatInputModule,
    MatDialogModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
