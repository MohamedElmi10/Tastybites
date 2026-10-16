import { Injectable, inject } from '@angular/core';
import {
  HttpBackend,
  HttpClient,
  HttpErrorResponse,
  HttpEvent,
  HttpHandler,
  HttpInterceptor,
  HttpRequest,
  HttpResponse,
} from '@angular/common/http';
import { Observable, map, of, shareReplay, throwError } from 'rxjs';
import type { RecipeDto } from '@proxy/recipes';

// Shape of one entry in recipe-seed-data.json (same file the backend's data seeder uses)
interface SeedRecipe {
  name: string;
  description: string;
  imageUrl?: string | null;
  ingredients: { name: string; amount?: number | null; unit?: string | null }[];
  steps: string[];
}

const ASSETS = '/static-demo';

// "Fake backend" for the static demo build: answers the API calls the public site makes,
// using JSON files shipped with the site. Registered only when environment.staticDemo is true.
// why: components and services stay exactly the same as with the real API - only this file knows
@Injectable()
export class StaticDemoInterceptor implements HttpInterceptor {
  // why: an HttpClient built on HttpBackend skips all interceptors, so loading the JSON files can't loop back here
  private readonly files = new HttpClient(inject(HttpBackend));

  // Loaded once, then cached. Ids follow the seed order, so "recent" = highest id like in the real API.
  private readonly recipes$ = this.files.get<SeedRecipe[]>(`${ASSETS}/recipe-seed-data.json`).pipe(
    map(seed =>
      seed.map((r, index): RecipeDto => ({
        id: index + 1,
        name: r.name,
        description: r.description,
        imageUrl: r.imageUrl ?? undefined,
        ingredients: r.ingredients.map(i => ({ name: i.name, amount: i.amount ?? undefined, unit: i.unit ?? undefined })),
        steps: r.steps.map(text => ({ text })),
      }))
    ),
    shareReplay(1)
  );

  intercept(req: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    const path = new URL(req.url, window.location.origin).pathname;

    // ABP's startup calls: anonymous snapshots taken from the real API
    if (path === '/api/abp/application-configuration') return this.file('application-configuration.json');
    if (path === '/api/abp/application-localization') return this.file('application-localization.json');

    // Same rules as RecipeAppService (public, read-only)
    if (path === '/api/app/recipe') {
      return this.reply(this.recipes$.pipe(map(list => [...list].sort((a, b) => a.name.localeCompare(b.name)))));
    }
    if (path === '/api/app/recipe/recent') {
      const count = Math.min(Math.max(Number(req.params.get('count') ?? 3), 1), 12);
      return this.reply(this.recipes$.pipe(map(list => [...list].sort((a, b) => b.id - a.id).slice(0, count))));
    }
    const detail = path.match(/^\/api\/app\/recipe\/(\d+)$/);
    if (detail) {
      const id = Number(detail[1]);
      return this.recipes$.pipe(
        map(list => {
          const recipe = list.find(r => r.id === id);
          if (!recipe) throw new HttpErrorResponse({ status: 404, statusText: 'Not Found', url: req.url });
          return new HttpResponse({ status: 200, body: recipe });
        })
      );
    }

    // why: ABP's OAuth setup reads the login server's discovery document at startup; without an answer it
    // logs a console error on every page. A minimal document keeps it quiet - nobody logs in on this site.
    if (path === '/.well-known/openid-configuration') {
      const origin = window.location.origin;
      return this.reply(
        of({
          issuer: origin + '/',
          authorization_endpoint: origin + '/connect/authorize',
          token_endpoint: origin + '/connect/token',
          end_session_endpoint: origin + '/connect/logout',
          jwks_uri: origin + '/.well-known/jwks',
          response_types_supported: ['code'],
          subject_types_supported: ['public'],
          id_token_signing_alg_values_supported: ['RS256'],
        })
      );
    }
    if (path === '/.well-known/jwks') return this.reply(of({ keys: [] }));

    // There is no server behind the demo: any other API call (e.g. login) fails like an offline backend
    if (path.startsWith('/api/') || path.startsWith('/.well-known/') || path.startsWith('/connect/')) {
      return throwError(() => new HttpErrorResponse({ status: 503, statusText: 'Static demo', url: req.url }));
    }
    return next.handle(req); // everything else: normal files of the site (images, fonts, i18n)
  }

  private file(name: string): Observable<HttpEvent<unknown>> {
    return this.reply(this.files.get(`${ASSETS}/${name}`));
  }

  private reply(body$: Observable<unknown>): Observable<HttpEvent<unknown>> {
    return body$.pipe(map(body => new HttpResponse({ status: 200, body })));
  }
}
