import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-pwa-install',
  templateUrl: './pwa-install.component.html',
  styleUrls: ['./pwa-install.component.scss']
})
export class PwaInstallComponent implements OnInit {
  
  isStandalone: boolean = false;
  showInstructions: boolean = false;
  bannerDismissed: boolean = false;

  constructor() { }

  ngOnInit(): void {
    this.checkStandaloneMode();
    this.checkBannerDismissed();
  }

  /**
   * Verifica si la app está ejecutándose en modo standalone (PWA)
   */
  private checkStandaloneMode(): void {
    // Verificar múltiples métodos para detectar modo standalone
    const isStandaloneDisplay = window.matchMedia('(display-mode: standalone)').matches;
    const isNavigatorStandalone = (navigator as any).standalone === true;
    const isFromHomescreen = window.location.search.includes('homescreen=1');
    
    this.isStandalone = isStandaloneDisplay || isNavigatorStandalone || isFromHomescreen;
    
    console.log('PWA Install Component - Standalone check:');
    console.log('- Display mode standalone:', isStandaloneDisplay);
    console.log('- Navigator standalone:', isNavigatorStandalone);
    console.log('- From homescreen:', isFromHomescreen);
    console.log('- Final result:', this.isStandalone);
  }

  /**
   * Verifica si el banner fue previamente cerrado
   */
  private checkBannerDismissed(): void {
    const dismissed = localStorage.getItem('pwa-banner-dismissed');
    this.bannerDismissed = dismissed === 'true';
  }

  /**
   * Cierra el banner y guarda la preferencia
   */
  dismissBanner(): void {
    this.bannerDismissed = true;
    localStorage.setItem('pwa-banner-dismissed', 'true');
  }

  /**
   * Resetea el estado del banner (para testing)
   */
  resetBanner(): void {
    this.bannerDismissed = false;
    localStorage.removeItem('pwa-banner-dismissed');
  }

  /**
   * Obtiene información del dispositivo para debugging
   */
  getDeviceInfo(): any {
    return {
      userAgent: navigator.userAgent,
      platform: navigator.platform,
      isIOS: /iPad|iPhone|iPod/.test(navigator.userAgent),
      isAndroid: /Android/.test(navigator.userAgent),
      isSafari: /Safari/.test(navigator.userAgent) && !/Chrome/.test(navigator.userAgent),
      isChrome: /Chrome/.test(navigator.userAgent),
      screen: {
        width: window.screen.width,
        height: window.screen.height
      },
      viewport: {
        width: window.innerWidth,
        height: window.innerHeight
      }
    };
  }
} 