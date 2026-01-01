import { AfterViewInit, Component, ViewChild, OnInit } from '@angular/core'
import { CommonModule } from '@angular/common'
import { MatTableDataSource, MatTableModule } from '@angular/material/table'
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator'
import { MatSort, MatSortModule } from '@angular/material/sort'
import { MatFormFieldModule } from '@angular/material/form-field'
import { MatInputModule } from '@angular/material/input'
import { MatIconModule } from '@angular/material/icon'
import { MatButtonModule } from '@angular/material/button'
import { CategoryService, Category } from '../../../core/services/category-service'
import { MatDialog, MatDialogModule } from '@angular/material/dialog'
import { CategoryDialogComponent } from './category-dialog'
@Component({
  selector: 'app-categories',
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatFormFieldModule,
    MatInputModule,
    MatIconModule,
    MatButtonModule,
    MatDialogModule
  ],
  templateUrl: './categories.html',
  styleUrl: './categories.css'
})
export class CategoriesComponent implements OnInit, AfterViewInit {

  displayedColumns = ['id', 'name', 'actions']
  dataSource = new MatTableDataSource<Category>()

  @ViewChild(MatPaginator) paginator!: MatPaginator
  @ViewChild(MatSort) sort!: MatSort

   constructor(
    private categoryService: CategoryService,
    private dialog: MatDialog
  ) {}

  ngOnInit() {
    this.loadCategories()
  }

  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator
    this.dataSource.sort = this.sort
  }

  loadCategories() {
    this.categoryService.getAll().subscribe({
      next: res => this.dataSource.data = res,
      error: err => console.error(err)
    })
  }

  


  applyFilter(event: any) {
    const value = event.target.value.trim().toLowerCase()
    this.dataSource.filter = value
  }

  addCategory() {
    this.dialog.open(CategoryDialogComponent)
      .afterClosed()
      .subscribe(r => r && this.loadCategories())
  }

  editCategory(category: Category) {
    this.dialog.open(CategoryDialogComponent, { data: { category } })
      .afterClosed()
      .subscribe(r => r && this.loadCategories())
  }

  deleteCategory(category: Category) {
    if (!confirm(`Delete category "${category.name}"?`)) return
    this.categoryService.delete(category.id)
      .subscribe(() => this.loadCategories())
  }
}
