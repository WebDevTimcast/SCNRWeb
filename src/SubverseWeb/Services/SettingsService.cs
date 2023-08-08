using ON.Fragments.Settings;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.IO.Pipelines;
using ON.Authentication;
using Grpc.Core;
using ON.Settings;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace SubverseWeb.Services
{
    public class SettingsService
    {
        private readonly ILogger logger;
        private readonly ServiceNameHelper nameHelper;
        private readonly SettingsClient settingsClient;
        private SettingsPublicData publicData = null;

        public SettingsService(ServiceNameHelper nameHelper, SettingsClient settingsClient, ILogger<SettingsService> logger)
        {
            this.logger = logger;
            this.nameHelper = nameHelper;
            this.settingsClient = settingsClient;
        }

        public async Task<SettingsPublicData> GetSettings()
        {
            try
            {
                if (publicData != null)
                    return publicData;

                var client = new SettingsInterface.SettingsInterfaceClient(nameHelper.SettingsServiceChannel);
                var res = await client.GetPublicDataAsync(new());

                publicData = res.Public;

                return publicData;
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error pulling Settings");
                return null;
            }
        }

        public async Task<ModifyResponseErrorType> Modify(CMSPublicRecord vm, ONUser user)
        {
            if (vm == null)
                return ModifyResponseErrorType.UnknownError;

            var client = new SettingsInterface.SettingsInterfaceClient(nameHelper.SettingsServiceChannel);
            var res = await client.ModifyCMSPublicDataAsync(new() { Data = vm }, GetMetadata(user));

            return res.Error;
        }

        public async Task<ModifyResponseErrorType> Modify(NotificationOwnerRecord vm, ONUser user)
        {
            if (vm == null)
                return ModifyResponseErrorType.UnknownError;

            var client = new SettingsInterface.SettingsInterfaceClient(nameHelper.SettingsServiceChannel);
            var res = await client.ModifyNotificationOwnerDataAsync(new() { Data = vm }, GetMetadata(user));

            return res.Error;
        }

        public async Task<ModifyResponseErrorType> Modify(PersonalizationPublicRecord vm, ONUser user)
        {
            if (vm == null)
                return ModifyResponseErrorType.UnknownError;

            var client = new SettingsInterface.SettingsInterfaceClient(nameHelper.SettingsServiceChannel);
            var res = await client.ModifyPersonalizationPublicDataAsync(new() { Data = vm }, GetMetadata(user));

            return res.Error;
        }

        public async Task<ModifyResponseErrorType> Modify(SubscriptionPublicRecord vm, ONUser user)
        {
            if (vm == null)
                return ModifyResponseErrorType.UnknownError;

            var client = new SettingsInterface.SettingsInterfaceClient(nameHelper.SettingsServiceChannel);
            var res = await client.ModifySubscriptionPublicDataAsync(new() { Data = vm }, GetMetadata(user));

            return res.Error;
        }

        public async Task<ModifyResponseErrorType> Modify(SubscriptionOwnerRecord vm, ONUser user)
        {
            if (vm == null)
                return ModifyResponseErrorType.UnknownError;

            var client = new SettingsInterface.SettingsInterfaceClient(nameHelper.SettingsServiceChannel);
            var res = await client.ModifySubscriptionOwnerDataAsync(new() { Data = vm }, GetMetadata(user));

            return res.Error;
        }

        private Metadata GetMetadata(ONUser user)
        {
            var data = new Metadata();
            data.Add("Authorization", "Bearer " + user.JwtToken);

            return data;
        }
        public Task<List<CategoryRecord>> GetCategories()
        {
            var settings = settingsClient.PublicData;

            return Task.FromResult(settings?.CMS?.Categories?.ToList() ?? new());
        }

        public async Task<CategoryRecord> GetCategoryById(string id)
        {
            return (await GetCategories()).FirstOrDefault(c => c.CategoryId == id);
        }

        public async Task<CategoryRecord> GetCategoryBySlug(string slug)
        {
            return (await GetCategories()).FirstOrDefault(c => c.UrlStub == slug);
        }

        public async Task<List<CategoryRecord>> GetCategoriesByIds(params string[] ids)
        {
            List<CategoryRecord> ret = new List<CategoryRecord>();

            foreach (var id in ids)
            {
                var cat = await GetCategoryById(id);
                if (cat != null)
                    ret.Add(cat);
            }

            return ret;
        }


        public async Task<ChannelRecord> GetChannelById(string id)
        {
            return (await GetChannels()).FirstOrDefault(c => c.ChannelId == id);
        }

        public async Task<ChannelRecord> GetChannelBySlug(string slug)
        {
            return (await GetChannels()).FirstOrDefault(c => c.UrlStub == slug);
        }

        public Task<List<ChannelRecord>> GetChannels()
        {
            var settings = settingsClient.PublicData;

            return Task.FromResult(settings?.CMS?.Channels?.ToList() ?? new());
        }
    }
}
