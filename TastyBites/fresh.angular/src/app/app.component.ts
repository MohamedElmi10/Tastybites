import { Component } from '@angular/core';

// The public site uses its own simple shell (header, page, footer) instead of ABP's admin layout:
// visitors never log in, so there is no user menu, language menu or admin navigation.
@Component({
  selector: 'app-root',
  template: `
    <header class="site-header">
      <div class="site-wrap site-header-row">
        <a class="site-logo" routerLink="/">Tasty<span>Bites</span></a>
        <nav class="site-links">
          <a routerLink="/" routerLinkActive="on" [routerLinkActiveOptions]="{ exact: true }">Home</a>
          <a routerLink="/recipes" routerLinkActive="on">Recipes</a>
        </nav>
      </div>
    </header>

    <main>
      <router-outlet></router-outlet>
    </main>

    <footer class="site-footer">
      <div class="site-wrap site-footer-row">
        <span>TastyBites · recipes from around the world</span>
        <span>Built with Angular + ABP</span>
      </div>
    </footer>
  `,
})
export class AppComponent {}
