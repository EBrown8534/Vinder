using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Stack_Exchange_Voting_Utility.Utilities
{
    public class StackExchangeApiConfiguration
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Key { get; set; }

        public static string RedirectUri { get; } = "http://vinder.info/Account/SEOAuthConfirm";
        public static string EncodedRedirectUri => HttpUtility.UrlEncode(RedirectUri);

        private static System.Collections.Specialized.NameValueCollection _appSettings =
            System.Web.Configuration.WebConfigurationManager.AppSettings;

        private StackExchangeApiConfiguration(string clientId, string clientSecret, string key)
        {
            ClientId = clientId;
            ClientSecret = clientSecret;
            Key = key;
        }

        public static StackExchangeApiConfiguration Load(string prefix = "StackExchangeApi_") =>
            new StackExchangeApiConfiguration(
                _appSettings[prefix + nameof(ClientId)],
                _appSettings[prefix + nameof(ClientSecret)],
                _appSettings[prefix + nameof(Key)]);
    }
}