import { Component, OnInit } from '@angular/core';
import { GraficosService } from '../../services/graficos.service';

@Component({
  selector: 'app-graficos',
  templateUrl: './graficos.component.html',
  styleUrls: ['./graficos.component.scss']
})
export class GraficosComponent implements OnInit {
  usuarioId: number = 1; // Por defecto, mostramos el usuario 1 (se puede cambiar después)
  tiposDeDatos: string[] = [];
  tipoSeleccionado: string = '';
  resumenDatos: any = {};
  cargando: boolean = false;
  error: string | null = null;

  constructor(private graficosService: GraficosService) { }

  ngOnInit(): void {
    this.cargarTiposDeDatos();
    this.cargarResumenDatos();
  }

  cargarTiposDeDatos(): void {
    this.cargando = true;
    this.error = null;

    this.graficosService.getTiposDeDatos(this.usuarioId).subscribe({
      next: (tipos) => {
        this.tiposDeDatos = tipos;
        if (tipos.length > 0) {
          this.tipoSeleccionado = tipos[0];
        }
        this.cargando = false;
      },
      error: (err) => {
        console.error('Error al cargar los tipos de datos:', err);
        this.error = 'No se pudieron cargar los tipos de datos';
        this.cargando = false;
      }
    });
  }

  cargarResumenDatos(): void {
    this.cargando = true;
    this.error = null;

    this.graficosService.getResumenDatosVitales(this.usuarioId).subscribe({
      next: (resumen) => {
        this.resumenDatos = resumen;
        this.cargando = false;
      },
      error: (err) => {
        console.error('Error al cargar el resumen de datos:', err);
        this.error = 'No se pudo cargar el resumen de datos';
        this.cargando = false;
      }
    });
  }

  cambiarTipoDato(tipo: string): void {
    this.tipoSeleccionado = tipo;
  }
} 