import { Component } from '@angular/core';

// One component for both creating and editing a recipe.
// Edit mode is detected from the :id in the URL (Part 6).
@Component({
  selector: 'app-crud-recipe',
  templateUrl: './crud-recipe.component.html',
  styleUrls: ['./crud-recipe.component.scss'],
})
export class CrudRecipeComponent {}
