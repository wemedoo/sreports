using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Common.Extensions;
using AutoMapper;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using sReportsV2.Common.Enums;
using sReportsV2.Common.Helpers;
using sReportsV2.Domain.Extensions;
using sReportsV2.Domain.Services.Implementations;
using sReportsV2.Domain.Services.Interfaces;
using sReportsV2.DTOs.Common.DataOut;
using sReportsV2.DTOs.User.DataIn;
using sReportsV2.TokenStorage;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Security;
using sReportsV2.DTOs.DTOs.User.DTO;

namespace sReportsV2.Controllers
{
    public partial class UserController : Controller
    {
        protected IUserBLL userBLL;
        protected IOrganizationBLL organizationBLL;

        public UserController() { }

        public UserController(IUserBLL userBLL, IOrganizationBLL organizationBLL)
        {
            this.userBLL = userBLL;
            this.organizationBLL = organizationBLL;
        }
        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult Login(string returnUrl, bool isLogin = true)
        {

            ViewBag.IsLogin = isLogin;
            ViewBag.ReturnUrl = returnUrl != "/User/Logout" ? returnUrl : "/";
            string loginView = ConfigurationManager.AppSettings["LoginViewName"];

            return View(loginView);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(UserLoginDataIn userDataIn)
        {

            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            userDataIn = Ensure.IsNotNull(userDataIn, nameof(userDataIn));

            if (ModelState.IsValid)
            {
                UserDataOut userDataOut = userBLL.TryLoginUser(userDataIn);
                if(userDataOut == null)
                {
                    ModelState.AddModelError("General", "Invalid Username or Password");
                    return View(userDataIn);
                }

                SignInUser(userDataOut.GetClaims());

                if (userDataOut.Organizations == null || userDataOut.Organizations.Count == 0)
                {
                    return View("ChooseActiveOrganization", userDataOut);
                }

                    
                if (!string.IsNullOrEmpty(userDataIn.ReturnUrl))
                {
                    return Redirect(userDataIn.ReturnUrl);
                }
                return RedirectToAction("GetAll", "EpisodeOfCare");
            }

            ModelState.AddModelError("General", "Invalid Username or Password");

            return View(userDataIn);
        }

        [AllowAnonymous]
        [HttpGet]
        public void Logout()
        {
            if (Request.IsAuthenticated)
            {
                SignOut();    
            }
            Response.Redirect(Url.Action("Index", ConfigurationManager.AppSettings["DefaultController"], null));
        }

        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GeneratePassword(string email)
        {
            userBLL.GeneratePassword(email);
            return new HttpStatusCodeResult(HttpStatusCode.Created);
        }

        [AllowAnonymous]
        public void SignIn()
        {
            if (!Request.IsAuthenticated)
            {
                Response.SuppressFormsAuthenticationRedirect = true;

                HttpContext.GetOwinContext().Authentication.Challenge(
                    new AuthenticationProperties { RedirectUri = "/" },
                    OpenIdConnectAuthenticationDefaults.AuthenticationType);
            }
            else 
            {
                Response.Redirect(Url.Action("Index", "Home", null));
            }
        }

        [AllowAnonymous]
        public void SignInGoogle()
        {
            if (!Request.IsAuthenticated)
            {
                SignOut();
                Response.SuppressFormsAuthenticationRedirect = true;

                HttpContext.GetOwinContext().Authentication.Challenge(
                    new AuthenticationProperties { RedirectUri = "/" },
                    DefaultAuthenticationTypes.ApplicationCookie);
            }
            else
            {
                Response.Redirect(Url.Action("Index", "Home", null));
            }
        }
        public ActionResult Error(string message, string debug)
        {
            return RedirectToAction("Login", "User", new { ReturnUrl = "/" });
        }

        private void SignInUser(List<Claim> claims)
        {
            var claimsIdentity = new ClaimsIdentity(claims,
            CookieAuthenticationDefaults.AuthenticationType);

            //This uses OWIN authentication
            HttpContext.GetOwinContext().Authentication.SignIn(
                    new AuthenticationProperties { IsPersistent = false },
                   claimsIdentity);
        }

        private void SignOut() 
        {
            var tokenStore = new SessionTokenStore(null,
                        System.Web.HttpContext.Current, ClaimsPrincipal.Current);

            tokenStore.Clear();
            System.Web.HttpContext.Current.Session["userData"] = null;
            HttpContext.GetOwinContext().Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType, DefaultAuthenticationTypes.ApplicationCookie);

        }
    }
}