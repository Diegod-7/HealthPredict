import { Component, OnInit } from '@angular/core';
import { ReportesService } from '../../../services/reportes.service';
import { ActivatedRoute } from '@angular/router';
import { FormBuilder, FormGroup } from '@angular/forms';
import { AuthService } from '../../../services/auth.service';

@Component({
  selector: 'app-reportes',
  templateUrl: './reportes.component.html',
  styleUrls: ['./reportes.component.scss']
})
export class ReportesComponent implements OnInit {
  usuarioId: number = 0;
  reporteForm: FormGroup;
  loading: boolean = false;
  today: Date = new Date();
  errorMessage: string = '';
  showError: boolean = false;
  tiposDatos: string[] = [
    'Presión Arterial',
    'Frecuencia Cardíaca',
    'Temperatura',
    'Glucosa',
    'Oxígeno en Sangre'
  ];

  constructor(
    private reportesService: ReportesService,
    private route: ActivatedRoute,
    private fb: FormBuilder,
    private authService: AuthService
  ) {
    this.reporteForm = this.fb.group({
      tipoDato: ['']
    });
  }

  ngOnInit(): void {
    // Obtener el ID del usuario actual
    this.usuarioId = this.authService.getUserId();
  }

  generarReporte(): void {
    this.loading = true;
    this.showError = false;
    this.errorMessage = '';
    const tipoDato = this.reporteForm.get('tipoDato')?.value;

    this.reportesService.generarReporteDatosVitales(this.usuarioId, tipoDato)
      .subscribe({
        next: (pdfBlob: Blob) => {
          // Crear URL para el blob
          const url = window.URL.createObjectURL(pdfBlob);
          
          // Crear elemento <a> para descarga
          const a = document.createElement('a');
          a.href = url;
          a.download = `HealthPredict_Reporte_${new Date().toISOString().replace(/[:.]/g, '-')}.pdf`;
          
          // Hacer clic en el enlace para iniciar la descarga
          document.body.appendChild(a);
          a.click();
          
          // Limpiar
          window.URL.revokeObjectURL(url);
          document.body.removeChild(a);
          this.loading = false;
        },
        error: (error) => {
          console.error('Error al generar el reporte:', error);
          this.loading = false;
          this.showError = true;
          this.errorMessage = error.message || 'Error al generar el reporte. Por favor intente nuevamente.';
          
          // Verificar si es un error de librería faltante
          if (this.errorMessage.includes('servicio de generación de PDF no está disponible')) {
            this.errorMessage += ' Este es un problema del servidor que requiere atención del administrador.';
          }
        }
      });
  }
}
