namespace AzureActiveDirectoryBatchProcessing.Models
{
    /// <summary>
    /// Represents the password profile of a user.
    /// See documentation here : https://msdn.microsoft.com/en-us/library/azure/ad/graph/api/users-operations
    /// </summary>
    public class PasswordProfile
    {
        /// <summary>
        /// Gets or sets weither the user will have to change its password.
        /// </summary>
        public bool ForceChangePasswordNextLogin => false;

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        public string Password { get; set; }
    }
}
