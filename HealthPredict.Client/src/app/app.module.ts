import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ChartsModule } from '@rinminase/ng-charts';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { AlertasListComponent } from './components/alertas-list/alertas-list.component';
import { AlertaDetalleComponent } from './components/alerta-detalle/alerta-detalle.component';
import { GraficosComponent } from './components/graficos/graficos.component';
import { LineaTemporalComponent } from './components/graficos/linea-temporal/linea-temporal.component';
import { ResumenEstadisticasComponent } from './components/graficos/resumen-estadisticas/resumen-estadisticas.component';
import { ComparativaMensualComponent } from './components/graficos/comparativa-mensual/comparativa-mensual.component';

@NgModule({
  declarations: [
    AppComponent,
    DashboardComponent,
    AlertasListComponent,
    AlertaDetalleComponent,
    GraficosComponent,
    LineaTemporalComponent,
    ResumenEstadisticasComponent,
    ComparativaMensualComponent
  ],
  imports: [
    BrowserModule,
    CommonModule,
    AppRoutingModule,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    ChartsModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
