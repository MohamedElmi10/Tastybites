import { Component, OnInit, inject } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ToasterService } from '@abp/ng.theme.shared';
import { Observable, finalize } from 'rxjs';
import { RecipeAdminService, RecipeDto } from '@proxy/recipes';

// Same limits as RecipeConsts.cs on the backend
const NAME_MAX_LENGTH = 128;
const DESCRIPTION_MAX_LENGTH = 1000;
const IMAGE_URL_MAX_LENGTH = 512;

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
    // Optional photo link; must start with http:// or https:// when filled in
    imageUrl: ['', [Validators.maxLength(IMAGE_URL_MAX_LENGTH), Validators.pattern(/^https?:\/\/\S+$/i)]],
  });

  // Set when the browser can't load the image at the given address
  previewFailed = false;

  recipeId: number | null = null;
  loading = false;
  saving = false;

  get previewUrl(): string | null {
    const control = this.form.controls.imageUrl;
    return control.value && control.valid ? control.value.trim() : null;
  }

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
        this.form.patchValue({
          name: recipe.name,
          description: recipe.description,
          imageUrl: recipe.imageUrl ?? '',
        });
      });
  }

  save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const { name, description, imageUrl } = this.form.getRawValue();
    const input: RecipeDto = {
      name,
      description,
      imageUrl: imageUrl.trim() || undefined, // empty field = no image (sent as null)
    };
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
