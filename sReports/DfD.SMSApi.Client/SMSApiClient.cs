using DfD.SMSApi.Client.DTOs.DataIn;
using Newtonsoft.Json;
using RestSharp;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DfD.SMSApi.Client
{
    public class SMSApiClient
    {
        private RestClient client;
        private string apiKey;
        public SMSApiClient(string baseAPIUrl, string apiKey)
        {
            client = new RestClient(baseAPIUrl);
            this.apiKey = apiKey;
        }

        public ValidationResultDataIn ValidateToken(string url, string token)
        {
            var request = new RestRequest($"{url}/{token ?? string.Empty}", DataFormat.Json);
            request.AddParameter("Authorization", apiKey, ParameterType.HttpHeader);

            var response = client.Get(request);
            ValidationResultDataIn result = JsonConvert.DeserializeObject<ValidationResultDataIn>(response.Content);
            return result;
        }

        public async Task InvalidateToken(string url, string jsonData, int numOfTries = 0)
        {
            Log.Information($"Started sending Invalidate Token request. Url: {url}. Data: {jsonData}");
            Log.Information($"Try: {numOfTries + 1}");


            var request = new RestRequest(url, Method.POST);

            request.AddParameter("application/json; charset=utf-8", jsonData, ParameterType.RequestBody);
            request.AddParameter("Authorization", apiKey, ParameterType.HttpHeader);
            request.RequestFormat = DataFormat.Json;
            try
            {
                var cancellationTokenSource = new CancellationTokenSource();
                var response = await client.ExecuteAsync(request, cancellationTokenSource.Token);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Log.Information($"Request sent successfully. Try: {numOfTries + 1}");
                }
                else
                {
                    Log.Information($"Request failed. Info: {response.Content} . Try: {numOfTries + 1}");
                    if (numOfTries < 5)
                    {
                        await InvalidateToken(url, jsonData, ++numOfTries);
                    }
                    else
                    {
                        Log.Information($"Request was not sent successfully after 5 retries.");
                    }
                }

            }
            catch (Exception error)
            {
                Log.Error(error.Message);
            }
        }

        public async Task<bool> NotifySMSAPI(string url, string jsonData, int numOfTries = 0)
        {
            bool result = false;
            Log.Information($"Started sending request to SMS API. Url: {url}. Data: {jsonData}");
            Log.Information($"Try: {numOfTries + 1}");


            var request = new RestRequest(url, Method.POST);

            request.AddParameter("application/json; charset=utf-8", jsonData, ParameterType.RequestBody);
            request.AddParameter("Authorization", apiKey, ParameterType.HttpHeader);
            request.RequestFormat = DataFormat.Json;
            try
            {
                var cancellationTokenSource = new CancellationTokenSource();
                var response = await client.ExecuteAsync(request, cancellationTokenSource.Token);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Log.Information($"Request sent successfully. Try: {numOfTries + 1}");
                    result = true;
                }
                else
                {
                    Log.Information($"Request failed. Info: {response.Content} . Try: {numOfTries + 1}");
                    if (numOfTries < 5)
                    {
                        result = await NotifySMSAPI(url, jsonData, ++numOfTries);
                    }
                    else
                    {
                        Log.Information($"Request was not sent successfully after 5 retries.");
                    }
                }

            }
            catch (Exception error)
            {
                Log.Error(error.Message);
            }

            Log.Information($"Result: {result}");
            return result;
        }
    }
}
