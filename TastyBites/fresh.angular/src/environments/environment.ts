import { Environment } from '@abp/ng.core';

const baseUrl = 'http://localhost:4201'; // public site (admin app runs on 4200)

// true only in environment.static.ts (the backend-less demo build)
export const staticDemo = false;

export const environment = {
  production: false,
  application: {
    baseUrl,
    name: 'TastyBites',
    logoUrl: '',
  },
  oAuthConfig: {
    issuer: 'http://localhost:5001/',
    redirectUri: baseUrl,
    clientId: 'TastyBites_App',
    responseType: 'code',
    scope: 'offline_access TastyBites',
    requireHttps: false,
  },
  apis: {
    default: {
      url: 'http://localhost:5001',
      rootNamespace: 'TastyBites',
    },
  },
} as Environment;
