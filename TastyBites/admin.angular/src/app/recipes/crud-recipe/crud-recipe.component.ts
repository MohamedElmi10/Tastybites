import { Component, inject } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ToasterService } from '@abp/ng.theme.shared';
import { finalize } from 'rxjs';
import { RecipeAdminService, RecipeDto } from '@proxy/recipes';

// Same limits as RecipeConsts.cs on the backend
const NAME_MAX_LENGTH = 128;
const DESCRIPTION_MAX_LENGTH = 1000;

// One component for both creating and editing a recipe.
// Part 5: create. Part 6 adds edit mode, detected from the :id in the URL.
@Component({
  selector: 'app-crud-recipe',
  templateUrl: './crud-recipe.component.html',
  styleUrls: ['./crud-recipe.component.scss'],
})
export class CrudRecipeComponent {
  private readonly fb = inject(FormBuilder);
  private readonly recipeAdminService = inject(RecipeAdminService);
  private readonly toaster = inject(ToasterService);
  private readonly router = inject(Router);

  readonly nameMaxLength = NAME_MAX_LENGTH;
  readonly descriptionMaxLength = DESCRIPTION_MAX_LENGTH;

  // Reactive form: fields and rules are defined here in code, the template just binds to them
  readonly form = this.fb.nonNullable.group({
    name: ['', [Validators.required, Validators.maxLength(NAME_MAX_LENGTH)]],
    description: ['', [Validators.required, Validators.maxLength(DESCRIPTION_MAX_LENGTH)]],
  });

  saving = false;

  save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched(); // show every field's error at once
      return;
    }

    const input: RecipeDto = this.form.getRawValue();
    this.saving = true;

    this.recipeAdminService
      .create(input)
      .pipe(finalize(() => (this.saving = false))) // runs on success and on error
      .subscribe(() => {
        this.toaster.success('::Recipes:Saved');
        this.router.navigate(['/recipes/list']);
      });
  }
}
