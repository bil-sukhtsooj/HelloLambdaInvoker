using Amazon;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
using Amazon.Lambda;
using Amazon.Lambda.Model;
using Newtonsoft.Json;

namespace HelloLambdaInvoker
{
    class Program
    {
        // Main method
        static async Task Main(string[] args)
        {
            Payload payload = new Payload
            {
                UserName = "Bil"
            };
            string json = JsonConvert.SerializeObject(payload);
            var response = await InvokeLambda("hello-lambda-csharp-proto-HelloLambda", json);
            Console.WriteLine(response.StatusCode);
        }

        public static async Task<InvokeResponse> InvokeLambda(string functionName, string payload, string profileName = "bil.dev")
        {
            var config = new AmazonLambdaConfig() { RegionEndpoint = RegionEndpoint.USEast2 };
            var chain = new CredentialProfileStoreChain();
            AWSCredentials awsCredentials;
            if (chain.TryGetAWSCredentials(profileName, out awsCredentials))
            {
                using (var client = new AmazonLambdaClient(awsCredentials, config))
                {
                    Task<InvokeResponse> response = client.InvokeAsync(new InvokeRequest
                    {
                        FunctionName = functionName,
                        Payload = payload
                    });
                    return response.Result;
                }
            }
            else
            {
                throw new Exception($"Could not load AWS credentials for profile name '{profileName}'");
            }
        }
    }
}