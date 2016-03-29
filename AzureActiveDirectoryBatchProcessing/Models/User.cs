
namespace AzureActiveDirectoryBatchProcessing.Models
{
    /// <summary>
    /// Represents an Azure Active directory user.
    /// See documentation here : https://msdn.microsoft.com/en-us/library/azure/ad/graph/api/users-operations
    /// </summary>
    public class User
    {
        /// <summary>
        /// Gets or sets weither it is a security group.
        /// </summary>
        public bool AccountEnabled => true;

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the mail nick name.
        /// </summary>
        public string MailNickname { get; set; }

        /// <summary>
        /// Gets or sets the password profile.
        /// </summary>
        public PasswordProfile PasswordProfile { get; set; }

        /// <summary>
        /// Gets or sets the user principal name.
        /// </summary>
        public string UserPrincipalName { get; set; }
    }
}
