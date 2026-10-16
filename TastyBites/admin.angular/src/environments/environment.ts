import { Environment } from '@abp/ng.core';

const baseUrl = 'http://localhost:4200';

// The visitor site (fresh.angular). Separate export because ABP's Environment type has no field for it.
export const publicSiteUrl = 'http://localhost:4201';

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
