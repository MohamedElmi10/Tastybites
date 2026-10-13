import { Component, Input } from '@angular/core';
import { NgIf } from '@angular/common';
import { RouterLink } from '@angular/router';
import { RecipeDto } from '@proxy/recipes';
import { RecipePhotoComponent } from './recipe-photo.component';

// One recipe as a clickable card: photo, name, two lines of description.
@Component({
  selector: 'app-recipe-card',
  standalone: true,
  imports: [NgIf, RouterLink, RecipePhotoComponent],
  template: `
    <a class="card-link" [routerLink]="['/recipes', recipe.id]">
      <app-recipe-photo [url]="recipe.imageUrl" [name]="recipe.name ?? ''"></app-recipe-photo>
      <div class="body">
        <span class="tag" *ngIf="isNew">Just added</span>
        <h3>{{ recipe.name }}</h3>
        <p>{{ recipe.description }}</p>
        <span class="more">View recipe →</span>
      </div>
    </a>
  `,
  styleUrls: ['./recipe-card.component.scss'],
})
export class RecipeCardComponent {
  @Input({ required: true }) recipe!: RecipeDto;
  @Input() isNew = false;
}
