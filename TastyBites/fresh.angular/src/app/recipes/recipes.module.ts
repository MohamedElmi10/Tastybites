import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { SharedModule } from '../shared/shared.module';
import { RecipeCardComponent } from '../site/recipe-card.component';
import { RecipePhotoComponent } from '../site/recipe-photo.component';
import { RecipesListComponent } from './recipes-list/recipes-list.component';
import { RecipeDetailComponent } from './recipe-detail/recipe-detail.component';

const routes: Routes = [
  { path: '', component: RecipesListComponent },     // /recipes
  { path: ':id', component: RecipeDetailComponent }, // /recipes/9
];

@NgModule({
  declarations: [RecipesListComponent, RecipeDetailComponent],
  imports: [SharedModule, RouterModule.forChild(routes), RecipeCardComponent, RecipePhotoComponent],
})
export class RecipesModule {}
