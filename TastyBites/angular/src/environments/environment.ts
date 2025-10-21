import { Environment } from '@abp/ng.core';

const baseUrl = 'http://localhost:4200';

export const environment = {
  production: false,
  application: {
    baseUrl,
    name: 'TastyBites',
    logoUrl: '',
  },
  oAuthConfig: {
    issuer: 'https://localhost:44399/',
    redirectUri: baseUrl,
    clientId: 'TastyBites_App',
    responseType: 'code',
    scope: 'offline_access TastyBites',
    requireHttps: true,
  },
  apis: {
    default: {
      url: 'https://localhost:44399',
      rootNamespace: 'TastyBites',
    },
  },
} as Environment;
