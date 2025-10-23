export const environment = {
  production: false,
  apiBaseUrl: 'https://localhost:7212/api',
  azure: {
    clientId: '<AZURE_SPA_CLIENT_ID>',
    authority: 'https://login.microsoftonline.com/<TENANT_ID_OR_COMMON>',
    redirectUri: 'http://localhost:5029',
    accessScoped: ["api://<AZURE_API_CLIENT_ID>/access_as_user"],
    scoped: ["openid","profile","email","api://<AZURE_API_CLIENT_ID>/access_as_user"]
  },
};
