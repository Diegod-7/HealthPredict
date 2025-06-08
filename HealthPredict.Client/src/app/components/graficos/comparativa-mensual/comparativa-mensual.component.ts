import { Component, OnInit, Input, OnChanges, SimpleChanges } from '@angular/core';
import { GraficosService } from '../../../services/graficos.service';

@Component({
  selector: 'app-comparativa-mensual',
  templateUrl: './comparativa-mensual.component.html',
  styleUrls: ['./comparativa-mensual.component.scss']
})
export class ComparativaMensualComponent implements OnInit, OnChanges {
  @Input() usuarioId!: number;
  @Input() tipoDato!: string;
  
  datos: any = null;
  cargando: boolean = false;
  error: string | null = null;

  constructor(private graficosService: GraficosService) { }

  ngOnInit(): void {
    this.cargarDatos();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['tipoDato'] || changes['usuarioId']) {
      this.cargarDatos();
    }
  }

  cargarDatos(): void {
    if (!this.usuarioId || !this.tipoDato) {
      return;
    }

    this.cargando = true;
    this.error = null;

    this.graficosService.getComparativaMensual(this.usuarioId, this.tipoDato).subscribe({
      next: (resultado) => {
        this.datos = resultado;
        this.cargando = false;
      },
      error: (err) => {
        console.error('Error al cargar la comparativa mensual:', err);
        this.error = 'No se pudo cargar la comparativa mensual';
        this.cargando = false;
      }
    });
  }

  obtenerClaseVariacion(): string {
    if (!this.datos) return '';
    
    const variacion = this.datos.variacion;
    
    if (variacion > 0) {
      return 'text-danger'; // Aumento
    } else if (variacion < 0) {
      return 'text-success'; // Disminución
    } else {
      return 'text-info'; // Sin cambios
    }
  }

  obtenerIconoVariacion(): string {
    if (!this.datos) return '';
    
    const variacion = this.datos.variacion;
    
    if (variacion > 0) {
      return 'bi bi-arrow-up-circle-fill'; // Aumento
    } else if (variacion < 0) {
      return 'bi bi-arrow-down-circle-fill'; // Disminución
    } else {
      return 'bi bi-dash-circle-fill'; // Sin cambios
    }
  }
} 