import type { EntityDto } from '@abp/ng.core';

export interface RecipeDto extends EntityDto<number> {
  name: string;
  description: string;
  imageUrl?: string;
}
