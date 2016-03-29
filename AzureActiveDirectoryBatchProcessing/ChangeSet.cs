using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace AzureActiveDirectoryBatchProcessing
{
    /// <summary>
    /// Represents a change set in a batch request.
    /// </summary>
    public class ChangeSet
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeSet"/> class.
        /// </summary>
        public ChangeSet()
        {
            Content = new MultipartContent("mixed", "changeset_" + Guid.NewGuid());
        }

        /// <summary>
        /// Gets the content of the <see cref="ChangeSet"/>.
        /// </summary>
        public MultipartContent Content { get; }

        /// <summary>
        /// Add an operation to the content of the <see cref="ChangeSet"/>.
        /// </summary>
        /// <param name="request">The request to add as an operation.</param>
        public void AddOperation(HttpRequestMessage request)
        {
            var operationContent = new HttpMessageContent(request);
            operationContent.Headers.ContentType = new MediaTypeHeaderValue("application/http");
            operationContent.Headers.Add("Content-Transfer-Encoding", "binary");
            Content.Add(operationContent);
        }
    }
}
