using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SteamWebAPI2.Exceptions;
using SteamWebAPI2.Models;
using SteamWebAPI2.Utilities;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using WhatsMySteamId.Models;

namespace WhatsMySteamId.Controllers
{
    public class HomeController : Controller
    {
        public static IConfiguration Configuration { get; set; }

        public HomeController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // GET: Home
        public ActionResult Index()
        {
            return View(new HomeIndexViewModel());
        }

        [HttpPost]
        public async Task<ActionResult> Index(HomeIndexViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string steamWebApiKey = Configuration["SteamWebApiKey"];
                    var webInterfaceFactory = new SteamWebInterfaceFactory(steamWebApiKey);

                    var steamId = webInterfaceFactory.CreateSteamWebInterface<SteamId>(new HttpClient());
                    await steamId.ResolveAsync(model.SearchString);
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
    }
}