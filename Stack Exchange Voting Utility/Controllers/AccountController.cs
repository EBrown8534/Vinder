using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Stack_Exchange_Voting_Utility.Models;
using System.Net;
using Stack_Exchange_Voting_Utility.Utilities;
using System.IO;

namespace Stack_Exchange_Voting_Utility.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View(new LoginViewModel());
        }

        private string GetAccessToken(string code)
        {
            var config = StackExchangeApiConfiguration.Load();

            var request = new Evbpc.Framework.Integrations.StackExchange.OAuth.Requests.AccessTokenRequest();
            request.ClientId = config.ClientId;
            request.ClientSecret = config.ClientSecret;
            request.RedirectUri = StackExchangeApiConfiguration.EncodedRedirectUri;
            request.Code = code;

            var apiConfig = new Evbpc.Framework.Integrations.StackExchange.OAuth.Configuration() { ClientId = config.ClientId, ClientSecret = config.ClientSecret, Key = config.Key };
            var handler = new Evbpc.Framework.Integrations.StackExchange.OAuth.Handler(apiConfig);
            var response = handler.SubmitRequest(request);

            return response;
        }

        private Evbpc.Framework.Integrations.StackExchange.API.Models.User GetUserInformation(string accessToken)
        {
            var config = StackExchangeApiConfiguration.Load();
            var apiConfig = new Evbpc.Framework.Integrations.StackExchange.API.Configuration()
            {
                Key = config.Key,
                AccessToken = accessToken
            };

            var request = new Evbpc.Framework.Integrations.StackExchange.API.Requests.MeAssociatedRequest() { PageSize = 100, Types = Evbpc.Framework.Integrations.StackExchange.API.Requests.MeAssociatedRequest.AccountTypes.MainSite };
            var handler = new Evbpc.Framework.Integrations.StackExchange.API.Handler(apiConfig);
            var response = handler.SubmitRequest(request);

            var myUsers = handler.ProcessResponse<Evbpc.Framework.Integrations.StackExchange.API.Models.NetworkUser>(response).Items;

            var meRequest = new Evbpc.Framework.Integrations.StackExchange.API.Requests.MeRequest() { Site = myUsers[0].SiteUrl.Substring(7) };
            var user = handler.ProcessResponse<Evbpc.Framework.Integrations.StackExchange.API.Models.User>(handler.SubmitRequest(meRequest)).Items[0];
            user.Reputation = myUsers.Sum(x => x.Reputation);

            return user;
        }

        [AllowAnonymous]
        public ActionResult SEOAuthConfirm(string code, string state)
        {
            var accessTokenResponse = GetAccessToken(code).Split('&');
            var accessToken = accessTokenResponse[0].Split('=')[1];
            var expiration = accessTokenResponse[1].Split('=')[1];
            var user = GetUserInformation(accessToken);

            using (var db = new ApplicationDbContext())
            {
                var accountId = Convert.ToInt64(user.AccountId);

                if (db.Users.Any(x => x.StackExchangeAccountId == accountId))
                {
                    var myUser = db.Users.First(x => x.StackExchangeAccountId == accountId);
                    myUser.AccessToken = accessToken;
                    myUser.AccessTokenExpiration = Evbpc.Framework.Utilities.Extensions.DateTimeExtensions.FromEpoch(Convert.ToInt64(expiration));
                    myUser.DisplayName = user.DisplayName;
                    myUser.Reputation = user.Reputation ?? 0;
                    myUser.AvatarUrl = user.ProfileImage;
                    db.SaveChanges();

                    SignInManager.SignIn(myUser, true, true);
                }
                else
                {
                    var myUser = new ApplicationUser();

                    myUser.StackExchangeAccountId = user.AccountId;
                    myUser.UserName = $"{user.AccountId}@stackexchange.local";
                    myUser.Email = $"{user.AccountId}@stackexchange.local";

                    myUser.AccessToken = accessToken;
                    myUser.AccessTokenExpiration = Evbpc.Framework.Utilities.Extensions.DateTimeExtensions.FromEpoch(Convert.ToInt64(expiration));
                    myUser.DisplayName = user.DisplayName;
                    myUser.Reputation = user.Reputation ?? 0;
                    myUser.AvatarUrl = user.ProfileImage;

                    var result = UserManager.Create(myUser);
                    SignInManager.SignIn(myUser, true, true);
                }

                return RedirectToAction("Index", "Home");
            }
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}