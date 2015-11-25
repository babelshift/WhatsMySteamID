using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WhatsMySteamId.Models
{
    public class HomeIndexViewModel
    {
        [Required(ErrorMessage = "You have to enter a non-empty value to search.")]
        public string SearchString { get; set; }

        public ulong SteamId64 { get; set; }
        public string CommunityProfileUrl { get; set; }
        public string LegacySteamId { get; set; }
        public string ModernSteamId { get; set; }
        public string Universe { get; set; }
        public string AccountType { get; set; }
        public string Instance { get; set; }
        public uint AccountId { get; set; }
        public bool IsValidSteamAccount { get; set; }
        public string ResolvedBy { get; set; }
    }
}