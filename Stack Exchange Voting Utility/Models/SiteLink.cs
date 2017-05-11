using Evbpc.Framework.Integrations.StackExchange.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Stack_Exchange_Voting_Utility.Models
{
    public class SiteLink
    {
        public NetworkUser User { get; set; }
        public Site Site { get; set; }
    }
}