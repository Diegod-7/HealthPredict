import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';

@Component({
  selector: 'app-resumen-estadisticas',
  templateUrl: './resumen-estadisticas.component.html',
  styleUrls: ['./resumen-estadisticas.component.scss']
})
export class ResumenEstadisticasComponent implements OnChanges {
  @Input() datos: any;
  
  ultimoValor: number = 0;
  promedio: number = 0;
  minimo: number = 0;
  maximo: number = 0;
  unidad: string = '';
  fecha: Date | null = null;

  constructor() { }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['datos'] && this.datos) {
      this.actualizarDatos();
    }
  }

  actualizarDatos(): void {
    this.ultimoValor = this.datos.ultimoValor;
    this.promedio = this.datos.promedio;
    this.minimo = this.datos.minimo;
    this.maximo = this.datos.maximo;
    this.unidad = this.datos.unidad;
    
    if (this.datos.fecha) {
      this.fecha = new Date(this.datos.fecha);
    }
  }

  obtenerTendencia(): string {
    if (this.ultimoValor > this.promedio) {
      return 'subiendo';
    } else if (this.ultimoValor < this.promedio) {
      return 'bajando';
    } else {
      return 'estable';
    }
  }

  obtenerIconoTendencia(): string {
    const tendencia = this.obtenerTendencia();
    switch (tendencia) {
      case 'subiendo':
        return 'bi bi-arrow-up-circle-fill text-danger';
      case 'bajando':
        return 'bi bi-arrow-down-circle-fill text-success';
      default:
        return 'bi bi-dash-circle-fill text-info';
    }
  }
} 