import { NgModule } from '@angular/core';
import { SharedModule } from '../shared/shared.module';
import { HomeRoutingModule } from './home-routing.module';
import { HomeComponent } from './home.component';
import { RecipeCardComponent } from '../site/recipe-card.component';
import { RecipePhotoComponent } from '../site/recipe-photo.component';

@NgModule({
  declarations: [HomeComponent],
  // Standalone components are imported, not declared
  imports: [SharedModule, HomeRoutingModule, RecipeCardComponent, RecipePhotoComponent],
})
export class HomeModule {}
