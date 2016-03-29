namespace AzureActiveDirectoryBatchProcessing
{
    internal class Constants
    {
        public const string TenantName = "mytenantname.onmicrosoft.com";
        public const string ClientId = "{Client Id of the application in the azure active directory}";
        public const string ClientSecret = "{Client Secret of the application in the azure active directory}";
        public const string ResourceUrl = "https://graph.windows.net/";
        public const string AuthString = "https://login.microsoftonline.com/" + TenantName;
        public const string TenantUrl = ResourceUrl + TenantName;
    }
}
