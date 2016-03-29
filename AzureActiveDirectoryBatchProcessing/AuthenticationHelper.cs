using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace AzureActiveDirectoryBatchProcessing
{
    internal class AuthenticationHelper
    {
        /// <summary>
        /// Get Token for Application.
        /// </summary>
        /// <returns>Token for application.</returns>
        public static string GetTokenForApplication()
        {
            return new AuthenticationContext(Constants.AuthString, false).AcquireToken(Constants.ResourceUrl,
                new ClientCredential(Constants.ClientId, Constants.ClientSecret)).AccessToken;
        }
    }
}
