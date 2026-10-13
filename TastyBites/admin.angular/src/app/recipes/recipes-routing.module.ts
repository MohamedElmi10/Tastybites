import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { RecipesListComponent } from './recipes-list/recipes-list.component';
import { CrudRecipeComponent } from './crud-recipe/crud-recipe.component';

// Child routes under /recipes (the parent route lives in app-routing.module.ts)
const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'list' },
  { path: 'list', component: RecipesListComponent },     // /recipes/list
  { path: 'create', component: CrudRecipeComponent },    // /recipes/create
  { path: 'edit/:id', component: CrudRecipeComponent },  // /recipes/edit/5 (same form, edit mode)
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class RecipesRoutingModule {}
