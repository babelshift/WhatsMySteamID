using SteamWebAPI2.Exceptions;
using SteamWebAPI2.Models;
using System;
using System.Configuration;
using System.Web.Mvc;
using WhatsMySteamId.Models;

namespace WhatsMySteamId.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View(new HomeIndexViewModel());
        }

        [HttpPost]
        public ActionResult Index(HomeIndexViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string steamWebApiKey = ConfigurationManager.AppSettings["steamWebApiKey"].ToString();
                    SteamId steamId = new SteamId(model.SearchString, steamWebApiKey);

                    ulong steamId64 = steamId.To64Bit();

                    model.AccountId = steamId.AccountId;
                    model.AccountType = steamId.AccountType.ToString();
                    model.Instance = steamId.InstanceId.ToString();
                    model.CommunityProfileUrl = String.Format("http://steamcommunity.com/profiles/{0}", steamId64);
                    model.LegacySteamId = steamId.ToLegacyFormat();
                    model.ModernSteamId = steamId.ToModernFormat();
                    model.SteamId64 = steamId64;
                    model.Universe = steamId.Universe.ToString();
                    model.IsValidSteamAccount = true;
                    model.ResolvedBy = steamId.ResolvedFrom.ToString();
                }
                catch (VanityUrlNotResolvedException)
                {
                    ModelState.AddModelError("e", "Could not find any Steam Account for your entered value.");
                }
                catch (InvalidSteamCommunityUriException)
                {
                    ModelState.AddModelError("e", "The URL that you provided isn't a valid Steam Community URL.");
                }
                catch (SteamIdNotConstructedException)
                {
                    ModelState.AddModelError("e", "The Steam ID couldn't be constructed based on your input.");
                }

                return View(model);
            }
            else
            {
                return RedirectToRoute("index");
            }
        }
    }
}