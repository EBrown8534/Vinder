using Microsoft.AspNet.Identity;
using Stack_Exchange_Voting_Utility.Models;
using Stack_Exchange_Voting_Utility.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SEAPI = Evbpc.Framework.Integrations.StackExchange.API;

namespace Stack_Exchange_Voting_Utility.Controllers
{
    public class HomeController : Controller
    {
        // Created on the SE API, should be permanent. This gives us some of the question properties that we don't get by default
        private string MainFilter = "!-MQ9xUObbPS8*asEAUIYfIkR2byR3b*M5";

        public ActionResult Index()
        {
            try
            {
                using (var db = new ApplicationDbContext())
                {
                    var user = db.Users.Find(User.Identity.GetUserId());
                    var users = GetAssociatedSites(user);

                    return View(new IndexViewModel(users));
                }
            }
            catch
            {
                // Possibly attempt to reauthenticate here
                return RedirectToAction("Login", "Account");
            }
        }

        public ActionResult ViewSite(string site)
        {
            ApplicationUser user = null;

            using (var db = new ApplicationDbContext())
            {
                user = db.Users.Find(User.Identity.GetUserId());

                if (user != null)
                {
                    try
                    {
                        var questions = GetQuestions(site, user.AccessToken);
                        SEAPI.Models.Question question = null;
                        int page = 1;

                        while (question == null)
                        {
                            page++;
                            questions = GetQuestions(site, user.AccessToken, page);
                            question = FindBestCandidate(questions, site, user, db);
                        }

                        return View(new ViewSiteViewModel(question, site));
                    }
                    catch (WebException)
                    {
                        // TODO: replace this with an attempt to reauthenticate through OAuth, the majority of the times this gets hit seem to be issues with the access_token needing to be revalidated
                        return RedirectToAction("Login", "Account");
                    }
                }
                else
                {
                    // Invalid user for some reason, force a login
                    return RedirectToAction("Login", "Account");
                }
            }
        }

        private List<SEAPI.Models.NetworkUser> GetAssociatedUsers(ApplicationUser user)
        {
            var apiConfig = StackExchangeApiConfiguration.Load();
            var config = new SEAPI.Configuration() { AccessToken = user.AccessToken, Key = apiConfig.Key };
            var request = new SEAPI.Requests.MeAssociatedRequest() { PageSize = 100, Types = SEAPI.Requests.MeAssociatedRequest.AccountTypes.MainSite };
            var handler = new SEAPI.Handler(config);
            var response = handler.ProcessResponse<SEAPI.Models.NetworkUser>(handler.SubmitRequest(request));
            return response.Items;
        }

        private List<SEAPI.Models.Site> GetSites()
        {
            var apiConfig = StackExchangeApiConfiguration.Load();
            var config = new SEAPI.Configuration() { Key = apiConfig.Key };
            var request = new SEAPI.Requests.SitesRequest() { PageSize = 1000 };
            var handler = new SEAPI.Handler(config);
            var response = handler.ProcessResponse<SEAPI.Models.Site>(handler.SubmitRequest(request));
            return response.Items;
        }

        private List<SiteLink> GetAssociatedSites(ApplicationUser user)
        {
            var users = GetAssociatedUsers(user);
            var sites = GetSites();

            return users.Select(x => new SiteLink { User = x, Site = sites.First(y => y.SiteUrl == x.SiteUrl) }).ToList();
        }

        private SEAPI.Models.Question FindBestCandidate(IEnumerable<SEAPI.Models.Question> questions, string site, ApplicationUser user, ApplicationDbContext db)
        {
            foreach (var question in questions)
            {
                if (!(question.Upvoted ?? false)
                    && !(question.Downvoted ?? false)
                    && question.Score <= 5
                    && question.CloseVoteCount == 0
                    && !question.ClosedDateTime.HasValue
                    && !db.UserQuestions.Any(x => x.UserId == user.Id && x.QuestionId == question.QuestionId && x.Site == site)
                    && question.Owner.Reputation <= 1000)
                {
                    return question;
                }
                else if ((question.Upvoted == true || question.Downvoted == true) && question.QuestionId.HasValue)
                {
                    // Track it in *our* DB so we don't waste time checking it against the SE API ever again.
                    db.UserQuestions.Add(new UserQuestionModel { Action = "Seen", QuestionId = question.QuestionId.Value, Site = site, UserId = user.Id });
                    db.SaveChanges();
                }
            }

            return null;
        }

        private List<SEAPI.Models.Question> GetQuestions(string site, string accessToken, int page = 1)
        {
            var apiConfig = StackExchangeApiConfiguration.Load();
            var config = new SEAPI.Configuration() { AccessToken = accessToken, Key = apiConfig.Key };
            var request = new SEAPI.Requests.QuestionsRequest()
            {
                Filter = MainFilter,
                Order = SEAPI.OrderType.Descending,
                Site = site,
                Sort = SEAPI.Requests.QuestionsRequest.SortType.Creation,
                Page = page,
                PageSize = 10,
            };
            var handler = new SEAPI.Handler(config);
            var response = handler.SubmitRequest(request);
            var responseType = handler.ProcessResponse<SEAPI.Models.Question>(response);
            return responseType.Items;
        }

        public ActionResult Vote(int id, string site, string direction)
        {
            using (var db = new ApplicationDbContext())
            {
                var user = db.Users.Find(User.Identity.GetUserId());

                switch (direction)
                {
                    case "Up":
                        // Submit a /questions/id/upvote API request to SE
                        try
                        {
                            var apiConfig = StackExchangeApiConfiguration.Load();
                            var config = new SEAPI.Configuration() { AccessToken = user.AccessToken, Key = apiConfig.Key };
                            var request = new SEAPI.Requests.VoteRequest { Filter = MainFilter, Id = id, Site = site, VoteAction = SEAPI.Requests.VoteRequest.VoteType.Upvote, Key = apiConfig.Key, AccessToken = user.AccessToken };
                            var handler = new SEAPI.Handler(config);
                            var result = handler.ProcessResponse<SEAPI.Models.Question>(handler.SubmitRequest(request));

                            user.Upvotes++;
                            db.SaveChanges();

                            if (!result.Items[0].Upvoted.Value)
                            {
                                // Failure, they either cannot upvote (15 rep?) or they are at their vote limit (40)
                                return RedirectToAction("Login", "Account");
                            }
                        }
                        catch (WebException)
                        {
                            // TODO: replace this with an attempt to reauthenticate through OAuth, the majority of the times this gets hit seem to be issues with the access_token needing to be revalidated
                            return RedirectToAction("Login", "Account");
                        }
                        break;
                    case "Skip":
                        // We don't really do anything for this. Adding the 'Skip' action to the DB handles it.
                        user.Skips++;
                        db.SaveChanges();
                        break;
                    case "Down":
                        // Submit a /questions/id/downvote API request to SE
                        try
                        {
                            var apiConfig = StackExchangeApiConfiguration.Load();
                            var config = new SEAPI.Configuration() { AccessToken = user.AccessToken, Key = apiConfig.Key };
                            var request = new SEAPI.Requests.VoteRequest { Filter = MainFilter, Id = id, Site = site, VoteAction = SEAPI.Requests.VoteRequest.VoteType.Downvote, Key = apiConfig.Key, AccessToken = user.AccessToken };
                            var handler = new SEAPI.Handler(config);
                            var result = handler.ProcessResponse<SEAPI.Models.Question>(handler.SubmitRequest(request));

                            user.Downvotes++;
                            db.SaveChanges();

                            if (!result.Items[0].Downvoted.Value)
                            {
                                // Failure, they either don't have 125 rep or they are at their vote limit (40)
                                return RedirectToAction("Login", "Account");
                            }
                        }
                        catch (WebException)
                        {
                            // TODO: replace this with an attempt to reauthenticate through OAuth, the majority of the times this gets hit seem to be issues with the access_token needing to be revalidated
                            return RedirectToAction("Login", "Account");
                        }
                        break;
                }

                // Action is **always** seen, it's possible to track what questions/answers a user upvotes/downvotes here but that takes away anonymity (sp?)
                // I think *at some point* I want to drop the 'Action' column altogether
                var userQuestionModel = new UserQuestionModel { UserId = user.Id, QuestionId = id, Site = site, Action = "Seen" };
                db.UserQuestions.Add(userQuestionModel);
                db.SaveChanges();
            }

            return RedirectToAction("ViewSite", new { site = site });
        }
    }
}
