import { Component, OnInit, inject } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ToasterService } from '@abp/ng.theme.shared';
import { Observable, finalize } from 'rxjs';
import { RecipeAdminService, RecipeDto, RecipeIngredientDto } from '@proxy/recipes';

// Same limits as RecipeConsts.cs on the backend
const NAME_MAX_LENGTH = 128;
const DESCRIPTION_MAX_LENGTH = 1000;
const IMAGE_URL_MAX_LENGTH = 512;
const INGREDIENT_NAME_MAX_LENGTH = 128;
const INGREDIENT_UNIT_MAX_LENGTH = 32;
const STEP_TEXT_MAX_LENGTH = 1000;
const MAX_INGREDIENTS = 50;
const MAX_STEPS = 50;
const AMOUNT_MAX = 99999999.99; // decimal(10,2) in the database

// why: Validators.required accepts "   "; the backend trims it to "" and would save an empty name
const NOT_BLANK = Validators.pattern(/\S/);

// One ingredient row in the form. Typed so getRawValue() gives real types, not `any`.
type IngredientForm = FormGroup<{
  name: FormControl<string>;
  amount: FormControl<number | null>;
  unit: FormControl<string>;
}>;

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
  readonly ingredientNameMaxLength = INGREDIENT_NAME_MAX_LENGTH;
  readonly ingredientUnitMaxLength = INGREDIENT_UNIT_MAX_LENGTH;
  readonly stepTextMaxLength = STEP_TEXT_MAX_LENGTH;
  readonly maxIngredients = MAX_INGREDIENTS;
  readonly maxSteps = MAX_STEPS;

  readonly form = this.fb.nonNullable.group({
    name: ['', [Validators.required, Validators.maxLength(NAME_MAX_LENGTH)]],
    description: ['', [Validators.required, Validators.maxLength(DESCRIPTION_MAX_LENGTH)]],
    // Optional photo: a full http(s) link, or /images/recipes/<file> for a photo hosted by our API.
    // Same rule as RecipeConsts.ImageUrlPattern on the backend.
    imageUrl: [
      '',
      [Validators.maxLength(IMAGE_URL_MAX_LENGTH), Validators.pattern(/^(https?:\/\/\S+|\/images\/recipes\/[\w.-]+)$/i)],
    ],
    // why: FormArray = a list of controls whose length changes at runtime.
    // The whole form is only valid when every row is valid.
    ingredients: this.fb.array<IngredientForm>([]),
    steps: this.fb.array<FormControl<string>>([]),
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

  get ingredients(): FormArray<IngredientForm> {
    return this.form.controls.ingredients;
  }

  get steps(): FormArray<FormControl<string>> {
    return this.form.controls.steps;
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
        // why: patchValue only fills rows that already exist, so the arrays are rebuilt row by row
        this.ingredients.clear();
        recipe.ingredients.forEach(ingredient => this.addIngredient(ingredient));
        this.steps.clear();
        recipe.steps.forEach(step => this.addStep(step.text));
      });
  }

  addIngredient(value?: RecipeIngredientDto): void {
    if (this.ingredients.length >= MAX_INGREDIENTS) return;
    this.ingredients.push(
      this.fb.nonNullable.group({
        name: [value?.name ?? '', [Validators.required, NOT_BLANK, Validators.maxLength(INGREDIENT_NAME_MAX_LENGTH)]],
        amount: this.fb.control<number | null>(value?.amount ?? null, [Validators.min(0), Validators.max(AMOUNT_MAX)]),
        unit: [value?.unit ?? '', Validators.maxLength(INGREDIENT_UNIT_MAX_LENGTH)],
      })
    );
  }

  addStep(text = ''): void {
    if (this.steps.length >= MAX_STEPS) return;
    this.steps.push(
      this.fb.nonNullable.control(text, [Validators.required, NOT_BLANK, Validators.maxLength(STEP_TEXT_MAX_LENGTH)])
    );
  }

  // Removes a row from either list
  remove(array: FormArray, index: number): void {
    array.removeAt(index);
  }

  // Moves a row one place up (-1) or down (+1). Works for both lists.
  // why: untyped FormArray param - these helpers never read row values, and generics fight Angular's typed-forms types
  // why: we move the same control object, so its value, validation state and DOM row move with it
  move(array: FormArray, index: number, direction: -1 | 1): void {
    const target = index + direction;
    if (target < 0 || target >= array.length) return;
    const control = array.at(index);
    array.removeAt(index);
    array.insert(target, control);
  }

  save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const { name, description, imageUrl, ingredients, steps } = this.form.getRawValue();
    // why: the API replaces both lists with what we send (docs/adr/0001), so always send the full lists
    const input: RecipeDto = {
      name,
      description,
      imageUrl: imageUrl.trim() || undefined, // empty field = no image (sent as null)
      ingredients: ingredients.map(i => ({
        name: i.name.trim(),
        amount: i.amount ?? undefined,
        unit: i.unit.trim() || undefined,
      })),
      steps: steps.map(text => ({ text: text.trim() })),
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
