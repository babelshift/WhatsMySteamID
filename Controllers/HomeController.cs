using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SteamWebAPI2.Exceptions;
using SteamWebAPI2.Models;
using WhatsMySteamID.Models;

namespace WhatsMySteamID.Controllers
{
    public class HomeController : Controller
    {
        public static IConfiguration Configuration { get; set; }

        public HomeController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public ActionResult Index()
        {
            return View(new HomeIndexViewModel());
        }

        [HttpPost]
        public IActionResult Index(HomeIndexViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string steamWebApiKey = Configuration.GetValue<string>("SteamWebApiKey");
                    SteamId steamId = new SteamId(model.SearchString, steamWebApiKey);

                    ulong steamId64 = steamId.To64Bit();

                    model.AccountId = steamId.AccountId;
                    model.AccountType = steamId.AccountType.ToString();
                    model.Instance = steamId.Instance.ToString();
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
