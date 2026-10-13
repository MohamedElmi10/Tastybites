import { Component, inject } from '@angular/core';
import { Observable, map } from 'rxjs';
import { RecipeDto, RecipeService } from '@proxy/recipes';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],
})
export class HomeComponent {
  private readonly recipeService = inject(RecipeService);

  // GET /api/app/recipe/recent?count=3 -> newest first (highest Id)
  readonly recent$: Observable<RecipeDto[]> = this.recipeService.getRecent(3);

  // GET /api/app/recipe -> only used for the "N recipes" count
  readonly total$: Observable<number> = this.recipeService.getList().pipe(map(list => list.length));
}
