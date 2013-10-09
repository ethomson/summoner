using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using Summoner.Util;

namespace Summoner.Rest
{
    public abstract class SummonerRestClient
    {
        public SummonerRestClient(SummonerRestClientConfiguration config)
        {
            Assert.NotNull(config, "config");
            Assert.NotNullOrEmpty(config.Uri, "config.Uri");

            this.Config = config;
        }

        public SummonerRestClientConfiguration Config
        {
            get;
            private set;
        }

        public T Execute<T>(IRestRequest request) where T : new()
        {
            RestClient client = new RestClient(Config.Uri);

            if (Config.UserAgent != null)
            {
                client.UserAgent = Config.UserAgent;
            }

            if (Config.Username != null)
            {
                client.Authenticator = new HttpBasicAuthenticator(Config.Username, Config.Password);
            }

            IRestResponse<T> response = client.Execute<T>(request);

            if (response.ErrorException != null)
            {
                throw response.ErrorException;
            }

            return response.Data;
        }

        public void Execute(IRestRequest request)
        {
            RestClient client = new RestClient(Config.Uri);

            if (Config.UserAgent != null)
            {
                client.UserAgent = Config.UserAgent;
            }

            if (Config.Username != null)
            {
                client.Authenticator = new HttpBasicAuthenticator(Config.Username, Config.Password);
            }

            IRestResponse response = client.Execute(request);

            if (response.ErrorException != null)
            {
                throw response.ErrorException;
            }
        }
    }
}
