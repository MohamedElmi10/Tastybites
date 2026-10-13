import { Component, OnInit, inject } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ToasterService } from '@abp/ng.theme.shared';
import { Observable, finalize } from 'rxjs';
import { RecipeAdminService, RecipeDto } from '@proxy/recipes';

// Same limits as RecipeConsts.cs on the backend
const NAME_MAX_LENGTH = 128;
const DESCRIPTION_MAX_LENGTH = 1000;

// One component for both creating and editing a recipe:
//   /recipes/create   -> create mode
//   /recipes/edit/5   -> edit mode for recipe 5
@Component({
  selector: 'app-crud-recipe',
  templateUrl: './crud-recipe.component.html',
  styleUrls: ['./crud-recipe.component.scss'],
})
export class CrudRecipeComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly route = inject(ActivatedRoute);
  private readonly recipeAdminService = inject(RecipeAdminService);
  private readonly toaster = inject(ToasterService);
  private readonly router = inject(Router);

  readonly nameMaxLength = NAME_MAX_LENGTH;
  readonly descriptionMaxLength = DESCRIPTION_MAX_LENGTH;

  readonly form = this.fb.nonNullable.group({
    name: ['', [Validators.required, Validators.maxLength(NAME_MAX_LENGTH)]],
    description: ['', [Validators.required, Validators.maxLength(DESCRIPTION_MAX_LENGTH)]],
  });

  recipeId: number | null = null;
  loading = false;
  saving = false;

  get isEditMode(): boolean {
    return this.recipeId !== null;
  }

  ngOnInit(): void {
    // Read :id from the URL. Missing on /recipes/create, so we stay in create mode.
    const id = this.route.snapshot.paramMap.get('id');
    if (id === null) return;

    this.recipeId = Number(id);
    this.loading = true;
    this.recipeAdminService
      .get(this.recipeId)
      .pipe(finalize(() => (this.loading = false)))
      .subscribe(recipe => {
        // Fill the form with the saved values
        this.form.patchValue({ name: recipe.name, description: recipe.description });
      });
  }

  save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const input: RecipeDto = this.form.getRawValue();
    const request$: Observable<RecipeDto> = this.isEditMode
      ? this.recipeAdminService.update(this.recipeId!, input)
      : this.recipeAdminService.create(input);

    this.saving = true;
    request$.pipe(finalize(() => (this.saving = false))).subscribe(() => {
      this.toaster.success('::Recipes:Saved');
      this.router.navigate(['/recipes/list']);
    });
  }
}
