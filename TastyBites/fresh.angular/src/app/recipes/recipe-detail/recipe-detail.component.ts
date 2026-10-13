import { Component, inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Observable, catchError, map, of, switchMap } from 'rxjs';
import { RecipeDto, RecipeService } from '@proxy/recipes';

// recipe = null means the API said 404 (or the id is invalid)
interface DetailState {
  recipe: RecipeDto | null;
}

@Component({
  selector: 'app-recipe-detail',
  templateUrl: './recipe-detail.component.html',
  styleUrls: ['./recipe-detail.component.scss'],
})
export class RecipeDetailComponent {
  private readonly route = inject(ActivatedRoute);
  private readonly recipeService = inject(RecipeService);

  // Re-runs when the :id in the URL changes (e.g. clicking a card under "More to cook")
  readonly state$: Observable<DetailState> = this.route.paramMap.pipe(
    map(params => Number(params.get('id'))),
    switchMap(id =>
      // skipHandleError: show our own "not found" page instead of ABP's error popup
      this.recipeService.get(id, { skipHandleError: true }).pipe(
        map(recipe => ({ recipe }) as DetailState),
        catchError(() => of({ recipe: null } as DetailState)),
      ),
    ),
  );

  // Three other recipes to keep visitors browsing
  readonly others$: Observable<RecipeDto[]> = this.route.paramMap.pipe(
    map(params => Number(params.get('id'))),
    switchMap(id => this.recipeService.getList().pipe(map(list => list.filter(r => r.id !== id).slice(0, 3)))),
  );
}
