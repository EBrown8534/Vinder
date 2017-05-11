using Evbpc.Framework.Integrations.StackExchange.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Stack_Exchange_Voting_Utility.Models
{
    public class IndexViewModel
    {
        public IndexViewModel(List<SiteLink> sites)
        {
            Sites = sites;
        }

        public List<SiteLink> Sites { get; set; }
    }

    public class ViewSiteViewModel
    {
        public ViewSiteViewModel(Question question, string site)
        {
            Id = question.QuestionId.Value;
            Site = site;
            Url = question.Link;
            Title = question.Title;
            Type = "Question";
            Tags = string.Join(", ", question.Tags.Select(x => $"[{x}]"));
            Views = question.ViewCount.Value;
            Created = question.CreationDateTime.Value;
            Body = question.Body;
            Author = question.Owner.DisplayName;
            AuthorUrl = question.Owner.Link;
        }

        public int Id { get; set; }
        public string Site { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public string Tags { get; set; }
        public int Views { get; set; }
        public DateTime Created { get; set; }
        public string Body { get; set; }
        public string Author { get; set; }
        public string AuthorUrl { get; set; }
    }
}