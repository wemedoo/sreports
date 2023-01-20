using Newtonsoft.Json;
using RestSharp;
using sReportsV2.Common.Exceptions;
using System.Collections.Generic;
using System.Net;
using UmlsClient.UMLSClasses;
using UMLSClient.UMLSClasses;

namespace UMLSClient.Client
{
    public class Client
    {
        private const string apiKey = "7e9e9b1b-cf72-4eea-9c86-55051d070bb8";
        private const string baseUrl = "https://uts-ws.nlm.nih.gov/rest/";

        public Client()
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        }

        public UMLSConceptDefinition GetConceptDefinition(string umlsCode)
        {
            UMLSConceptDefinition searchResult = null;

            if (HasCode(umlsCode))
            {
                string endpoint = $"content/current/CUI/{umlsCode}/definitions";

                IRestResponse response = GetResponse(endpoint);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    searchResult = JsonConvert.DeserializeObject<UMLSConceptDefinition>(response.Content);
                }
            }

            return searchResult;
        }

        public UMLSSemanticTypes GetSemanticTypes(string umlsCode)
        {
            UMLSSemanticTypes searchResult = null;

            if (HasCode(umlsCode))
            {
                string endpoint = GetEntityForUmlsCode(umlsCode).Result.SemanticTypes[0].Uri;
                IRestResponse response = GetResponse(endpoint);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    searchResult = JsonConvert.DeserializeObject<UMLSSemanticTypes>(response.Content);
                }
            }

            return searchResult;
        }

        public UMLSAtomResult GetAtomsResult(string umlsCode,int pageSize = 25, int pageNumber=1)
        {
            UMLSAtomResult searchResult = null;

            if (HasCode(umlsCode))
            {
                string endpoint = $"content/current/CUI/{umlsCode}/atoms";
                IRestResponse response = GetResponse(endpoint, GetSearchParams(pageNumber, pageSize));
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    searchResult = JsonConvert.DeserializeObject<UMLSAtomResult>(response.Content);
                }
            }

            return searchResult;
        }
        public UMLSSearchResult GetSearchResult(string searchString, int pageSize = 25, int pageNumber = 1)
        {
            string endpoint = "search/current";

            IRestResponse response = GetResponse(endpoint, GetSearchParams(pageNumber, pageSize, searchString));
            UMLSSearchResult searchResult = JsonConvert.DeserializeObject<UMLSSearchResult>(response.Content);

            return searchResult;
        }

        private UMLSConcept GetEntityForUmlsCode(string umlsCode)
        {
            string endpoint = $"content/current/CUI/{umlsCode}";

            IRestResponse response = GetResponse(endpoint);
            UMLSConcept concept = JsonConvert.DeserializeObject<UMLSConcept>(response.Content);

            return concept;
        }

        private bool HasCode(string umlsCode)
        {
            return !string.IsNullOrEmpty(umlsCode);
        }

        private IDictionary<string, object> GetSearchParams(int pageNumber, int pageSize, string searchWord = "")
        {
            IDictionary<string, object> searchParams = new Dictionary<string, object>() { 
                { "pageNumber", pageNumber }, 
                { "pageSize", pageSize } 
            };

            if (!string.IsNullOrEmpty(searchWord))
            {
                searchParams.Add("string", searchWord);
            }

            return searchParams;
        }

        private IRestResponse GetResponse(string endpoint, IDictionary<string, object> parameters = null)
        {
            RestClient client = new RestClient(baseUrl);
            RestRequest request = new RestRequest(endpoint, Method.GET);
            SetParameters(request, parameters);
            return Execute(client, request);
        }

        private void SetParameters(RestRequest request, IDictionary<string, object> parameters)
        {
            request.AddParameter("apiKey", apiKey);
            if(parameters != null)
            {
                foreach (KeyValuePair<string, object> parameter in parameters)
                {
                    request.AddParameter(parameter.Key, parameter.Value);
                }
            }
        }

        private IRestResponse Execute(RestClient client, RestRequest request)
        {
            IRestResponse restResponse = client.Execute(request);
            HandleResponseIfNotSuccessful(restResponse);

            return restResponse;
        }

        private void HandleResponseIfNotSuccessful(IRestResponse restResponse)
        {
            if (!restResponse.IsSuccessful && restResponse.StatusCode != HttpStatusCode.NotFound)
            {
                throw new ApiCallException($"Status code: {restResponse.StatusCode}, Response content: {restResponse.Content}, Response uri: {restResponse.ResponseUri}");
            }
        }
    }
}
