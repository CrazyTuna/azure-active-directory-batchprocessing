using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using AzureActiveDirectoryBatchProcessing.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace AzureActiveDirectoryBatchProcessing
{
    public class Program
    {
        // Single-Threaded Apartment required for OAuth2 Authz Code flow (User Authn) to execute for this demo app
        [STAThread]
        private static void Main()
        {
            Run().Wait();
        }

        private static async Task Run()
        {
            var token = AuthenticationHelper.GetTokenForApplication();

            // Create a group for test
            var groupId = await CreateGroupTest(token);

            // Create 100 users for test
            //await CreateUsersTest(token);

            // Get all the users
            var users = await GetAllUsers(token);

            await AddMemberToGroup(token, groupId,
                ((JArray) users["value"]).Select(t => t["objectId"].Value<string>()).ToList());
        }

        private static async Task<string> CreateGroupTest(string token)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var uid = Guid.NewGuid();
                var jsonGroup = SerializeObject(new Group()
                {
                    DisplayName = $"Test Group {uid}",
                    MailNickname = $"TestGroup{uid}"
                });

                var response = await
                    client.PostAsync($"{Constants.TenantUrl}/groups?api-version=1.6",
                        new StringContent(jsonGroup, Encoding.UTF8, "application/json"));
                return JObject.Parse(await response.Content.ReadAsStringAsync())["objectId"].Value<string>();
            }
        }

        /// <summary>
        /// This take a long time to process....
        /// </summary>
        private static async Task CreateUsersTest(string token)
        {
            for (var j = 0; j < 20; j++)
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                    // Create the multi part content
                    var boundary = "batch_" + Guid.NewGuid();
                    var mainContent = new MultipartContent("mixed", boundary);

                    for (var i = 0; i < 5; i++)
                    {
                        // Create a request to create a new user
                        var addUser = new HttpRequestMessage(HttpMethod.Post,
                            $"{Constants.TenantUrl}/users?api-version=1.6");

                        var userName = Guid.NewGuid();
                        var jsonUser = SerializeObject(new User
                        {
                            MailNickname = $"usertest{userName}",
                            DisplayName = $"user test {userName}",
                            PasswordProfile = new PasswordProfile
                            {
                                Password = "Test1234"
                            },
                            UserPrincipalName = $"usertest{userName}@{Constants.TenantName}"
                        });

                        addUser.Content = new StringContent(jsonUser, Encoding.UTF8, "application/json");
                        var subboundary = "changeset_" + Guid.NewGuid();
                        var subContent = new MultipartContent("mixed", subboundary);
                        var addUsercontent = new HttpMessageContent(addUser);
                        addUsercontent.Headers.ContentType = new MediaTypeHeaderValue("application/http");
                        addUsercontent.Headers.Add("Content-Transfer-Encoding", "binary");
                        subContent.Add(addUsercontent);
                        mainContent.Add(subContent);
                    }

                    var batchRequest = new HttpRequestMessage(HttpMethod.Post,
                        $"{Constants.TenantUrl}/$batch?api-version=1.6");
                    batchRequest.Content = mainContent;
                    await client.SendAsync(batchRequest);
                }
        }

        private static async Task<JObject> GetAllUsers(string token)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await client.GetAsync($"{Constants.TenantUrl}/users?api-version=1.6");
                var json = await response.Content.ReadAsStringAsync();
                return JObject.Parse(json);
            }
        }
        
        private static async Task AddMemberToGroup(string token, string groupId, IList<string> memberIds)
        {
            // MembersIds are 
            if (memberIds.Count > 100)
            {
                // A batch can contain up to 5 changesets. Each changeset can contain up to 20 operations.
                throw new InvalidOperationException("Cannot send more than 100 operation in an batch");
            }

            var batch = new BatchRequest(Constants.TenantUrl);

            // A changeset can contain up to 20 operations
            var takeCount = 20;
            var skipCount = 0;
            var take = memberIds.Skip(skipCount).Take(takeCount).ToList();
            while (take.Count > 0)
            {
                AddChangeset(batch, groupId, take);
                skipCount += takeCount;
                take = memberIds.Skip(skipCount).Take(takeCount).ToList();
            }

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                var requestContent = batch.Request.Content.ReadAsStringAsync();
                var response = await client.SendAsync(batch.Request);

                // Only if you want to parse the response...
                var contentString = await response.Content.ReadAsStringAsync();
            }
        }

        private static void AddChangeset(BatchRequest batch, string groupId, IEnumerable<string> memberIds)
        {
            var changeset = batch.AddChangeSet();
            foreach (var memberId in memberIds)
            {
                // Create the HttpRequest to add a member to a group
                var request = AddMemberToGroupRequest(groupId, memberId);

                // Add the operation to the changeset
                changeset.AddOperation(request);
            }
        }

        private static HttpRequestMessage AddMemberToGroupRequest(string groupId, string memberId)
        {
            // Create a request to add a member to a group
            var request = new HttpRequestMessage(HttpMethod.Post,
                $"{Constants.TenantUrl}/groups/{groupId}/$links/members?api-version=1.6");

            // Create the body of the request
            var jsonBody = SerializeObject(new DirectoryObject
            {
                Url = $"{Constants.TenantUrl}/directoryObjects/{memberId}"
            });

            // Set the content
            request.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            // Return the request
            return request;
        }

        private static string SerializeObject(object obj)
        {
            return JsonConvert.SerializeObject(obj,
                new JsonSerializerSettings {ContractResolver = new CamelCasePropertyNamesContractResolver()});
        }
    }
}
