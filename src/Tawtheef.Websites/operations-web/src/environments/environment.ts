export const environment = {
  production: false,
  apiBaseUrl: 'https://localhost:7212/api',
  azure: {
    clientId: 'e8de85c6-a8ce-4b63-b2be-019a21ea074d',
    authority: 'https://login.microsoftonline.com/2dcae639-d4a4-4454-82c7-592ab66fc7bd',
    redirectUri: 'http://localhost:7212',
    accessScoped: ["api://e8de85c6-a8ce-4b63-b2be-019a21ea074d/access_as_user"],
    scoped: ["openid","profile","email","api://e8de85c6-a8ce-4b63-b2be-019a21ea074d/access_as_user"]
  }
};
