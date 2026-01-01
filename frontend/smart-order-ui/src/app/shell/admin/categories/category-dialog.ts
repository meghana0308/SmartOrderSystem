import { Component, Inject, OnInit } from '@angular/core'
import { CommonModule } from '@angular/common'
import { FormsModule } from '@angular/forms'
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog'
import { MatFormFieldModule } from '@angular/material/form-field'
import { MatInputModule } from '@angular/material/input'
import { MatButtonModule } from '@angular/material/button'
import { CategoryService } from '../../../core/services/category-service'

@Component({
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule
  ],
  templateUrl: './category-dialog.html'
})
export class CategoryDialogComponent implements OnInit {
  name = ''

  constructor(
    private dialogRef: MatDialogRef<CategoryDialogComponent>,
    private categoryService: CategoryService,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {}

  ngOnInit() {
    if (this.data?.category) {
      this.name = this.data.category.name
    }
  }

  save() {
    if (!this.name.trim()) return

    const payload = { name: this.name.trim() }

    if (this.data?.category) {
      this.categoryService.update(this.data.category.id, payload)
        .subscribe(() => this.dialogRef.close(true))
    } else {
      this.categoryService.create(payload)
        .subscribe(() => this.dialogRef.close(true))
    }
  }

  cancel() {
    this.dialogRef.close()
  }
}
