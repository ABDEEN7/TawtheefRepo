export const environment = {
  production: false,
  apiBaseUrl: 'https://localhost:7072/api',
  azure: {
    clientId: '6b888b0f-ebbb-4443-b104-e2ef3ad58b4f',
    authority: 'https://login.microsoftonline.com/2dcae639-d4a4-4454-82c7-592ab66fc7bd',
    redirectUri: 'http://localhost:7072',
    accessScoped: ["api://6b888b0f-ebbb-4443-b104-e2ef3ad58b4f/access_as_user"],
    scoped: ["openid","profile","email","api://6b888b0f-ebbb-4443-b104-e2ef3ad58b4f/access_as_user"]
  }
};
