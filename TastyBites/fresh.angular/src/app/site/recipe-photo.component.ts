import { Component, Input } from '@angular/core';
import { NgIf } from '@angular/common';
import { environment } from '../../environments/environment';

// Shows the recipe's photo, or one shared placeholder when there is no ImageUrl
// or the link is broken. Standalone component: no NgModule needed, just import it where used.
@Component({
  selector: 'app-recipe-photo',
  standalone: true,
  imports: [NgIf],
  template: `
    <div class="photo" [class.placeholder]="!showImage">
      <img *ngIf="showImage" [src]="src" [alt]="name" loading="lazy" (error)="failed = true" />
      <ng-container *ngIf="!showImage">
        <svg viewBox="0 0 120 80" fill="none" stroke="currentColor" stroke-width="3" stroke-linecap="round" aria-hidden="true">
          <circle cx="60" cy="40" r="26" />
          <circle cx="60" cy="40" r="16" />
          <path d="M18 14v18m-6-18v12a6 6 0 0 0 12 0V14M18 38v30" />
          <path d="M102 14c-8 4-10 16-10 24h10M102 14v54" />
        </svg>
        <span>No photo yet</span>
      </ng-container>
    </div>
  `,
  styleUrls: ['./recipe-photo.component.scss'],
})
export class RecipePhotoComponent {
  @Input() url?: string | null;
  @Input() name = '';

  failed = false;

  get showImage(): boolean {
    return !!this.url && !this.failed;
  }

  // "/images/recipes/x.jpg" is a photo hosted by our API -> prefix the API address; full links stay as they are.
  // why: the database stores self-hosted photos as a relative path so it survives a domain change
  // (the admin app has the same rule in its ImageSrcPipe)
  get src(): string | null {
    if (!this.url) return null;
    return this.url.startsWith('/') ? environment.apis.default.url + this.url : this.url;
  }
}
