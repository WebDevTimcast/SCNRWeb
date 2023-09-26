﻿using ON.Authentication;
using ON.Fragments.Settings;
using ON.Settings;
using SCNRWeb.Services;
using System.Threading.Tasks;

namespace SCNRWeb.Models.SiteSettings
{
    public class IndexViewModel
    {
        public SettingsPublicData Public { get; set; }
        public SettingsPrivateData Private { get; set; }
        public SettingsOwnerData Owner { get; set; }

        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }

        public IndexViewModel() { }

        public static IndexViewModel Load(SettingsClient settingsClient, ONUser user)
        {
            if (!user.IsAdminOrHigher)
                return new();

            var vm = new IndexViewModel()
            {
                Private = settingsClient.PrivateData,
                Public = settingsClient.PublicData,
            };

            if (user.IsOwner)
                vm.Owner = settingsClient.OwnerData;

            return vm;
        }
    }
}
