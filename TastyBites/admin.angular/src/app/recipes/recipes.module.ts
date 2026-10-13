import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { NgxDatatableModule } from '@swimlane/ngx-datatable';
import { SharedModule } from '../shared/shared.module';
import { RecipesRoutingModule } from './recipes-routing.module';
import { RecipesListComponent } from './recipes-list/recipes-list.component';
import { CrudRecipeComponent } from './crud-recipe/crud-recipe.component';

// Feature module for everything recipe-related in the admin app.
// Loaded lazily: the code is only downloaded when the admin opens /recipes.
@NgModule({
  declarations: [RecipesListComponent, CrudRecipeComponent],
  imports: [SharedModule, ReactiveFormsModule, NgxDatatableModule, RecipesRoutingModule],
})
export class RecipesModule {}
