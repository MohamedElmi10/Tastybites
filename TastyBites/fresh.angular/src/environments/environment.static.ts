import { Environment } from '@abp/ng.core';

// Static demo build (ng build -c static), e.g. for Netlify: no backend.
// StaticDemoInterceptor answers every /api call from JSON shipped with the site.
// why: the site's own address is read at runtime, so the same build works on any domain
const baseUrl = window.location.origin;

export const staticDemo = true;

export const environment = {
  production: true,
  application: {
    baseUrl,
    name: 'TastyBites',
    logoUrl: '',
  },
  oAuthConfig: {
    // Visitors never log in on the public site; this only keeps ABP's auth setup happy
    issuer: baseUrl + '/',
    redirectUri: baseUrl,
    clientId: 'TastyBites_App',
    responseType: 'code',
    scope: 'offline_access TastyBites',
    requireHttps: false,
  },
  apis: {
    default: {
      url: '', // same origin: /api/... is caught by the interceptor, /images/... are files of this site
      rootNamespace: 'TastyBites',
    },
  },
} as Environment;
