using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using UMLSClient.UMLSClasses;

namespace UMLSClient.Client
{
    public class Client
    {
        private string apiKey = "33b11d2b-6b80-4855-87aa-5793b3ea101d";
        private string baseUrl = "https://uts-ws.nlm.nih.gov/rest/";
        public TGT Tgt { get; set; }

        public Client()
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            this.Tgt = this.GetTGT();
        }

        public Client(string apiKey)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            this.Tgt = this.GetTGT();
            this.apiKey = apiKey;
        }
        public UMLSConceptDefinition GetConceptDefinition(string umlsCode,int pageNumber= 1, int pageSize = 25)
        {
            UMLSConceptDefinition searchResult = null;
            string ticket = this.GetServiceTicket();
            string endpoint = $"content/current/CUI/{umlsCode}/definitions?ticket={ticket}";
                                  
            IRestResponse response = GetResponse(endpoint);
            if(response.StatusCode == HttpStatusCode.OK)
            {
                searchResult = JsonConvert.DeserializeObject<UMLSConceptDefinition>(response.Content);
            }

            return searchResult;
        }

        public UMLSAtomResult GetAtomsResult(string umlsCode,int pageSize = 25, int pageNumber=1)
        {
            UMLSAtomResult searchResult = null;
            string ticket = this.GetServiceTicket();
            string endpoint = $"content/current/CUI/{umlsCode}/atoms?ticket={ticket}&pageSize={pageSize}&pageNumber={pageNumber}";

            IRestResponse response = GetResponse(endpoint);
            if(response.StatusCode == HttpStatusCode.OK)
            {
                searchResult = JsonConvert.DeserializeObject<UMLSAtomResult>(response.Content);
            }

            return searchResult;
        }
        public UMLSSearchResult GetSearchResult(string searchString, int pageSize = 25, int pageNumber = 1)
        {
            string ticket = this.GetServiceTicket();
            string endpoint = $"search/current?string={searchString}&ticket={ticket}&pageSize={pageSize}&pageNumber={pageNumber}";

            IRestResponse response = GetResponse(endpoint);
            UMLSSearchResult searchResult = JsonConvert.DeserializeObject<UMLSSearchResult>(response.Content);

            return searchResult;
        }
        public UMLSConcept GetEntityForUmlsCode(string umlsCode)
        {
            string ticket = this.GetServiceTicket();
            string endpoint = $"content/current/CUI/{umlsCode}?ticket={ticket}";
            
            IRestResponse response = GetResponse(endpoint);
            UMLSConcept concept = JsonConvert.DeserializeObject<UMLSConcept>(response.Content);

            return concept;
        }

        public string GetServiceTicket()
        {
            SetTGTIfExpired();
            var client = new RestClient(Tgt.TGTValue);
            var request = new RestRequest("", Method.POST);
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("service", "http://umlsks.nlm.nih.gov");
            IRestResponse response = client.Execute(request);

            return response.Content;
        }

        public TGT GetTGT()
        {
            var client = new RestClient(@"https://utslogin.nlm.nih.gov/");
            var request = new RestRequest("cas/v1/api-key", Method.POST);
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("apikey", apiKey);
            IRestResponse response = client.Execute(request);
            string result = response.Content.Substring(response.Content.IndexOf("\"https:") + 1, response.Content.IndexOf("method=") - response.Content.IndexOf("\"https:") - 3);

            return new TGT() { StartTime = DateTime.Now, TGTValue = result };
        }

        private void SetTGTIfExpired()
        {
            if (DateTime.Now.Hour - Tgt.StartTime.Hour > 7)
            {
                Tgt = this.GetTGT();
            }
        }

        private IRestResponse GetResponse(string endpoint)
        {
            var client = new RestClient(baseUrl);
            var request = new RestRequest(endpoint, Method.GET);
            return client.Execute(request); ;
        }
    }
}
