using System;
using System.Net.Http;

namespace AzureActiveDirectoryBatchProcessing
{
    /// <summary>
    /// Represents a batch request.
    /// </summary>
    public class BatchRequest
    {
        private readonly MultipartContent _batchContent;

        /// <summary>
        /// Initializes a new instance of the <see cref="BatchRequest"/> class.
        /// </summary>
        public BatchRequest(string tenantUrl)
        {
            // Create the batch request
            Request = new HttpRequestMessage(HttpMethod.Post, $"{tenantUrl}/$batch?api-version=1.6");

            // Initializes the batch content
            _batchContent = new MultipartContent("mixed", "batch_" + Guid.NewGuid());
            Request.Content = _batchContent;
        }

        /// <summary>
        /// Gets the request to send.
        /// </summary>
        public HttpRequestMessage Request { get; }

        /// <summary>
        /// Add a new changeset to the batch.
        /// </summary>
        public ChangeSet AddChangeSet()
        {
            // Create a new changeset
            var changeSet = new ChangeSet();

            // Add the content of the changeset to the batch
            _batchContent.Add(changeSet.Content);

            // return the changeset
            return changeSet;
        }
    }
}
