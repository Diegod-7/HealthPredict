import { Component, OnInit, OnDestroy } from '@angular/core';
import { PasosService, PasosHoy } from '../../services/pasos.service';
import { Subscription, interval } from 'rxjs';

@Component({
  selector: 'app-pasos-chart',
  templateUrl: './pasos-chart.component.html',
  styleUrls: ['./pasos-chart.component.scss']
})
export class PasosChartComponent implements OnInit, OnDestroy {
  pasosHoy: PasosHoy | null = null;
  cargando = false;
  error: string | null = null;
  usuarioId = 7; // Usuario fijo como en el backend
  private subscription?: Subscription;
  private refreshSubscription?: Subscription;

  // Configuración del gráfico
  chartOptions = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: {
        display: false
      },
      title: {
        display: true,
        text: 'Pasos por Hora - Hoy',
        font: {
          size: 16,
          weight: 'bold'
        }
      }
    },
    scales: {
      y: {
        beginAtZero: true,
        ticks: {
          stepSize: 100
        },
        title: {
          display: true,
          text: 'Pasos'
        }
      },
      x: {
        title: {
          display: true,
          text: 'Hora del día'
        }
      }
    }
  };

  chartData = {
    labels: [] as string[],
    datasets: [{
      label: 'Pasos',
      data: [] as number[],
      backgroundColor: 'rgba(54, 162, 235, 0.6)',
      borderColor: 'rgba(54, 162, 235, 1)',
      borderWidth: 2,
      borderRadius: 4,
      borderSkipped: false,
    }]
  };

  constructor(private pasosService: PasosService) { }

  ngOnInit(): void {
    this.cargarPasosHoy();
    
    // Actualizar cada 5 minutos
    this.refreshSubscription = interval(5 * 60 * 1000).subscribe(() => {
      this.cargarPasosHoy();
    });
  }

  ngOnDestroy(): void {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
    if (this.refreshSubscription) {
      this.refreshSubscription.unsubscribe();
    }
  }

  cargarPasosHoy(): void {
    this.cargando = true;
    this.error = null;

    this.subscription = this.pasosService.getPasosHoy(this.usuarioId).subscribe({
      next: (data) => {
        this.pasosHoy = data;
        this.actualizarGrafico();
        this.cargando = false;
      },
      error: (error) => {
        console.error('Error cargando pasos:', error);
        this.error = 'Error al cargar los datos de pasos';
        this.cargando = false;
      }
    });
  }

  private actualizarGrafico(): void {
    if (!this.pasosHoy) return;

    // Crear un array de todas las horas del día
    const todasLasHoras = Array.from({ length: 24 }, (_, i) => i);
    
    const labels = todasLasHoras.map(hora => `${hora.toString().padStart(2, '0')}:00`);
    const data = todasLasHoras.map(hora => {
      const datoHora = this.pasosHoy!.datosGrafico.find(d => d.hora === hora);
      return datoHora ? datoHora.pasos : 0;
    });

    this.chartData = {
      labels: labels,
      datasets: [{
        label: 'Pasos',
        data: data,
        backgroundColor: 'rgba(76, 175, 80, 0.6)',
        borderColor: 'rgba(76, 175, 80, 1)',
        borderWidth: 2,
        borderRadius: 4,
        borderSkipped: false,
      }]
    };
  }

  getMetaDelDia(): number {
    return 10000; // Meta típica de 10,000 pasos por día
  }

  getPorcentajeMeta(): number {
    if (!this.pasosHoy) return 0;
    return Math.min(100, (this.pasosHoy.totalPasos / this.getMetaDelDia()) * 100);
  }

  getColorPorcentaje(): string {
    const porcentaje = this.getPorcentajeMeta();
    if (porcentaje >= 100) return '#4CAF50'; // Verde
    if (porcentaje >= 75) return '#FF9800'; // Naranja
    if (porcentaje >= 50) return '#FFC107'; // Amarillo
    return '#F44336'; // Rojo
  }

  formatearFecha(fecha: string): string {
    const fechaObj = new Date(fecha);
    return fechaObj.toLocaleDateString('es-ES', {
      weekday: 'long',
      year: 'numeric',
      month: 'long',
      day: 'numeric'
    });
  }

  formatearHora(fecha: string): string {
    const fechaObj = new Date(fecha);
    return fechaObj.toLocaleTimeString('es-ES', {
      hour: '2-digit',
      minute: '2-digit'
    });
  }
} 