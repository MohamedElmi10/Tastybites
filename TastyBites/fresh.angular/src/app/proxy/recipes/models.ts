import type { EntityDto } from '@abp/ng.core';

export interface RecipeDto extends EntityDto<number> {
  name: string;
  description: string;
  imageUrl?: string;
  ingredients: RecipeIngredientDto[];
  steps: RecipeStepDto[];
}

export interface RecipeIngredientDto {
  name: string;
  amount?: number;
  unit?: string;
}

export interface RecipeStepDto {
  text: string;
}
