import type { RecipeDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class RecipeService {
  apiName = 'Default';
  

  get = (id: number, config?: Partial<Rest.Config>) =>
    this.restService.request<any, RecipeDto>({
      method: 'GET',
      url: `/api/app/recipe/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getList = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, RecipeDto[]>({
      method: 'GET',
      url: '/api/app/recipe',
    },
    { apiName: this.apiName,...config });
  

  getRecent = (count: number = 3, config?: Partial<Rest.Config>) =>
    this.restService.request<any, RecipeDto[]>({
      method: 'GET',
      url: '/api/app/recipe/recent',
      params: { count },
    },
    { apiName: this.apiName,...config });

  constructor(private restService: RestService) {}
}
