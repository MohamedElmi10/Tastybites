import { Pipe, PipeTransform } from '@angular/core';
import { environment } from '../../environments/environment';

// Turns a recipe's imageUrl into something an <img> can load:
//   "https://…"                -> unchanged (photo on another site)
//   "/images/recipes/x.jpg"    -> "http://localhost:5001/images/recipes/x.jpg" (photo hosted by our API)
// why: the database stores self-hosted photos as a relative path so it survives a domain change;
// the API's address lives in environment.ts, so only the frontend knows where to prefix it.
// The public site has the same rule in recipe-photo.component.ts (separate app, no shared library).
@Pipe({ name: 'imageSrc', standalone: true })
export class ImageSrcPipe implements PipeTransform {
  transform(url: string | null | undefined): string | null {
    if (!url) return null;
    return url.startsWith('/') ? environment.apis.default.url + url : url;
  }
}
