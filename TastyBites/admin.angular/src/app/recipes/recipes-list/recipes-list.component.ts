import { Component, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { RecipeAdminService, RecipeDto } from '@proxy/recipes';

@Component({
  selector: 'app-recipes-list',
  templateUrl: './recipes-list.component.html',
  styleUrls: ['./recipes-list.component.scss'],
})
export class RecipesListComponent {
  // Generated proxy service: a typed wrapper around GET /api/app/recipe-admin/recipes
  private readonly recipeAdminService = inject(RecipeAdminService);

  // An Observable is a stream that delivers the recipes once the API answers.
  // The `async` pipe in the template subscribes for us and unsubscribes when the page closes.
  readonly recipes$: Observable<RecipeDto[]> = this.recipeAdminService.getAllRecipes();

  // Start sorted by name, A to Z. Clicking a column header changes it.
  readonly defaultSort = [{ prop: 'name', dir: 'asc' }];
}
