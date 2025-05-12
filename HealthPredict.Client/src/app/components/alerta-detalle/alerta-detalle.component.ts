import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AlertaService } from '../../services/alerta.service';
import { Alerta } from '../../models/alerta.model';

@Component({
  selector: 'app-alerta-detalle',
  templateUrl: './alerta-detalle.component.html',
  styleUrls: ['./alerta-detalle.component.scss']
})
export class AlertaDetalleComponent implements OnInit {
  alerta: Alerta | null = null;
  alertaId: number = 0;
  loading = false;
  error: string | null = null;
  resolucionForm: FormGroup;
  modoResolucion = false;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private alertaService: AlertaService,
    private fb: FormBuilder
  ) {
    this.resolucionForm = this.fb.group({
      notasResolucion: ['', [Validators.required, Validators.minLength(5)]]
    });
  }

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      this.alertaId = +params['id'];
      this.cargarAlerta();
    });
  }

  cargarAlerta(): void {
    this.loading = true;
    this.error = null;

    this.alertaService.getAlerta(this.alertaId).subscribe({
      next: (data) => {
        this.alerta = data;
        this.loading = false;
        
        // Si no está leída, marcarla como leída
        if (this.alerta && !this.alerta.leida) {
          this.marcarComoLeida();
        }
      },
      error: (err) => {
        this.error = 'Error al cargar la alerta: ' + err.message;
        this.loading = false;
      }
    });
  }

  marcarComoLeida(): void {
    if (!this.alerta || this.alerta.leida) return;
    
    this.alertaService.marcarComoLeida(this.alertaId).subscribe({
      next: (alertaActualizada) => {
        this.alerta = alertaActualizada;
      },
      error: (err) => {
        console.error('Error al marcar como leída:', err);
      }
    });
  }

  toggleModoResolucion(): void {
    this.modoResolucion = !this.modoResolucion;
  }

  resolverAlerta(): void {
    if (this.resolucionForm.invalid) {
      this.resolucionForm.markAllAsTouched();
      return;
    }

    const notasResolucion = this.resolucionForm.get('notasResolucion')?.value;
    
    this.alertaService.resolverAlerta(this.alertaId, notasResolucion).subscribe({
      next: (alertaActualizada) => {
        this.alerta = alertaActualizada;
        this.modoResolucion = false;
      },
      error: (err) => {
        this.error = 'Error al resolver la alerta: ' + err.message;
      }
    });
  }

  volver(): void {
    this.router.navigate(['/alertas']);
  }

  getSeveridadClass(severidad: string): string {
    switch(severidad.toLowerCase()) {
      case 'alta': return 'badge bg-danger';
      case 'media': return 'badge bg-warning';
      case 'baja': return 'badge bg-info';
      default: return 'badge bg-secondary';
    }
  }
} 