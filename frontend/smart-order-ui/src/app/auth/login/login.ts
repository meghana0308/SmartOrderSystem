import { Component } from '@angular/core'
import { FormBuilder, Validators, ReactiveFormsModule } from '@angular/forms'
import { Router } from '@angular/router'
import { CommonModule } from '@angular/common'
import { AuthService } from '../../core/services/auth-service'

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './login.html'
})
export class LoginComponent {

  form
  error = ''

  constructor(
    private readonly fb: FormBuilder,
    private readonly auth: AuthService,
    private readonly router: Router
  ) {
    this.form = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', Validators.required]
    })
  }

  login() {
    if (this.form.invalid) {
      this.error = 'Please enter valid email and password'
      return
    }

    const payload = {
      email: this.form.controls.email.value!,
      password: this.form.controls.password.value!
    }

    this.auth.login(payload).subscribe({
      next: res => {
        this.auth.saveToken(res.token)

        

        const role = this.auth.getRole()
              console.log('ROLE AFTER LOGIN:', role)


        switch (role) {
          case 'Admin': this.router.navigate(['/admin']); break
          case 'SalesExecutive': this.router.navigate(['/sales']); break
          case 'WarehouseManager': this.router.navigate(['/warehouse']); break
          case 'FinanceOfficer': this.router.navigate(['/finance']); break
          default: this.router.navigate(['/customer'])
        }
      },
      error: () => {
        this.error = 'Invalid email or password'
      }
    })
  }
}
