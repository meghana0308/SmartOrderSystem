import { Component, OnInit, inject } from '@angular/core'
import { RouterModule, Router } from '@angular/router'
import { AuthService } from './core/services/auth-service'
import { CommonModule, isPlatformBrowser } from '@angular/common'
import { PLATFORM_ID } from '@angular/core'

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './app.html'
})
export class App {}
