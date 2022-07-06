export default class AppSettings {
    public static clientId: string = "b0b67ea-9f30-4ca4-b7df-e2d8a229fa8c";
  public static tenant: string = "common";
  // this is WebAPI URL for local development
  public static apiRoot: string = "https://localhost:44302/api/";
  public static apiScopes: string[] = [
    "api://" + AppSettings.clientId + "/Reports.Embed"
  ];
}