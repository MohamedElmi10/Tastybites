import { Component, inject } from '@angular/core';
import { RecipeDto, RecipeService } from '@proxy/recipes';

@Component({
  selector: 'app-recipes-list',
  templateUrl: './recipes-list.component.html',
  styleUrls: ['./recipes-list.component.scss'],
})
export class RecipesListComponent {
  private readonly recipeService = inject(RecipeService);

  recipes: RecipeDto[] | null = null; // null = still loading
  search = '';

  constructor() {
    // GET /api/app/recipe (already sorted A to Z by the API)
    this.recipeService.getList().subscribe(list => (this.recipes = list));
  }

  // Search runs in the browser on the loaded list: no extra API calls
  get filtered(): RecipeDto[] {
    const term = this.search.trim().toLowerCase();
    if (!this.recipes || !term) return this.recipes ?? [];
    return this.recipes.filter(r => `${r.name} ${r.description}`.toLowerCase().includes(term));
  }
}
