import { Component } from '@angular/core'
import { FormBuilder, Validators, ReactiveFormsModule } from '@angular/forms'
import { Router } from '@angular/router'
import { CommonModule } from '@angular/common'
import { AuthService } from '../../core/services/auth-service'

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './register.html'
})
export class RegisterComponent {

  form
  error = ''
  success = ''

  constructor(
    private readonly fb: FormBuilder,
    private readonly auth: AuthService,
    private readonly router: Router
  ) {
    this.form = this.fb.group({
      fullName: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      password: ['', Validators.required]
    })
  }

 register(event?: Event) {
  if (event) event.preventDefault(); 
  if (this.form.invalid) return

  const payload = {
    fullName: this.form.controls.fullName.value!,
    email: this.form.controls.email.value!,
    password: this.form.controls.password.value!
  }

  this.auth.register(payload).subscribe({
    next: () => {
      //this.success = 'Registration successful. Redirecting to login...'
      this.router.navigateByUrl('/login') // Direct navigation
    },
    error: err => {
      this.error = err?.error?.message || 'Registration failed'
    }
  })
}

}
