using SteamWebAPI2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
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
        public async Task<ActionResult> Index(HomeIndexViewModel model)
        {
            // try to parse to a 64 bit int
            // if successful, user may have entered the steamid64
            ulong steamId64 = 0;
            bool isSteamId64 = ulong.TryParse(model.SearchString, out steamId64);
            model.ResolvedBy = "64-bit Steam ID";

            if (!isSteamId64)
            {
                // otherwise try to parse a url
                // if successful, user may have entered a community profile url
                Uri uriResult;
                bool result = Uri.TryCreate(model.SearchString, UriKind.Absolute, out uriResult)
                    && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

                SteamWebAPI2.Interfaces.SteamUser steamUser = new SteamWebAPI2.Interfaces.SteamUser("TO DO, ADD STEAM KEY TO PROTECTED CONFIG AND AZURE CONFIG");
                if (result)
                {
                    model.CommunityProfileUrl = model.SearchString;

                    if(uriResult.Segments.Count() == 3)
                    {
                        string profileId = uriResult.Segments[2];

                        // try to parse the end as a 64 bit int
                        isSteamId64 = ulong.TryParse(profileId, out steamId64);

                        // if not successful, use the end as the profile name
                        if(!isSteamId64)
                        {
                            // TODO: Handle if this responds with an error
                            steamId64 = await steamUser.ResolveVanityUrlAsync(profileId);
                        }

                        model.ResolvedBy = "Steam Community Profile URL";
                    }
                }
                else
                {
                    // TODO: Handle if this responds with an error
                    // not a url and not a 64-bit int, just use the value entered
                    steamId64 = await steamUser.ResolveVanityUrlAsync(model.SearchString);
                    model.ResolvedBy = "Steam Community Vanity Profile Name";
                }
            }

            if (steamId64 > 0)
            {
                SteamId steamId = new SteamId(steamId64);

                model.AccountId = steamId.AccountId;
                model.AccountType = steamId.AccountType.ToString();
                model.Instance = steamId.InstanceId.ToString();
                model.CommunityProfileUrl = String.Format("http://steamcommunity.com/profiles/{0}", steamId64);
                model.LegacySteamId = steamId.ToLegacyFormat();
                model.ModernSteamId = steamId.ToModernFormat();
                model.SteamId64 = steamId64;
                model.Universe = steamId.Universe.ToString();
                model.IsValidSteamAccount = true;
            }
            else
            {
                model.IsValidSteamAccount = false;
            }

            return View(model);
        }
    }
}