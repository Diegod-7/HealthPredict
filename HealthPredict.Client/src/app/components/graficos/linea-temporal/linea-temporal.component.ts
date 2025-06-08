import { Component, OnInit, Input, OnChanges, SimpleChanges } from '@angular/core';
import { ChartConfiguration, ChartOptions } from 'chart.js';
import { GraficosService } from '../../../services/graficos.service';

@Component({
  selector: 'app-linea-temporal',
  templateUrl: './linea-temporal.component.html',
  styleUrls: ['./linea-temporal.component.scss']
})
export class LineaTemporalComponent implements OnInit, OnChanges {
  @Input() usuarioId!: number;
  @Input() tipoDato!: string;
  
  public lineChartData: ChartConfiguration<'line'>['data'] = {
    labels: [],
    datasets: [
      {
        data: [],
        label: '',
        fill: false,
        tension: 0.2,
        borderColor: 'rgba(78, 115, 223, 1)',
        backgroundColor: 'rgba(78, 115, 223, 0.2)',
        pointBackgroundColor: 'rgba(78, 115, 223, 1)',
        pointBorderColor: '#fff',
        pointHoverBackgroundColor: '#fff',
        pointHoverBorderColor: 'rgba(78, 115, 223, 1)'
      }
    ]
  };
  
  public lineChartOptions: ChartOptions<'line'> = {
    responsive: true,
    maintainAspectRatio: false,
    scales: {
      x: {
        ticks: {
          maxRotation: 45,
          minRotation: 45
        }
      }
    },
    plugins: {
      legend: {
        display: true
      }
    }
  };

  cargando: boolean = false;
  error: string | null = null;
  unidad: string = '';

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

    this.graficosService.getDatosVitalesPorTipo(this.usuarioId, this.tipoDato).subscribe({
      next: (datos) => {
        if (datos && datos.length > 0) {
          this.unidad = datos[0].unidad;
          
          // Configurar las etiquetas y datos para el gráfico
          this.lineChartData.labels = datos.map(d => new Date(d.fecha).toLocaleDateString());
          this.lineChartData.datasets[0].data = datos.map(d => d.valor);
          this.lineChartData.datasets[0].label = `${this.tipoDato} (${this.unidad})`;
          
          // Actualizar el objeto completo para que Chart.js detecte los cambios
          this.lineChartData = { ...this.lineChartData };
        }
        this.cargando = false;
      },
      error: (err) => {
        console.error('Error al cargar los datos:', err);
        this.error = 'No se pudieron cargar los datos para el gráfico';
        this.cargando = false;
      }
    });
  }
} 