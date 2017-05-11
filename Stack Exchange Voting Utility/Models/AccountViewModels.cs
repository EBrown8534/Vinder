using Stack_Exchange_Voting_Utility.Utilities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace Stack_Exchange_Voting_Utility.Models
{
    public class LoginViewModel
    {
        public StackExchangeApiConfiguration Config { get; set; } = StackExchangeApiConfiguration.Load();
        
        public string Url => "https://" + $"stackexchange.com/oauth/?client_id={Config.ClientId}&scope=write_access,private_info&redirect_uri={StackExchangeApiConfiguration.EncodedRedirectUri}";
    }
}
