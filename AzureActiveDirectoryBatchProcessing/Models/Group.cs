namespace AzureActiveDirectoryBatchProcessing.Models
{
    /// <summary>
    /// Represents an Azure Active directory Group.
    /// See documentation here : https://msdn.microsoft.com/en-us/library/azure/ad/graph/api/groups-operations
    /// </summary>
    public class Group
    {
        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the mail nickname.
        /// </summary>
        public string MailNickname { get; set; }

        /// <summary>
        /// Gets or sets weither the mail is enabled.
        /// </summary>
        public bool MailEnabled => false;

        /// <summary>
        /// Gets or sets weither it is a security group.
        /// </summary>
        public bool SecurityEnabled => true;
    }
}
