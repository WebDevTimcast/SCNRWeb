using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Http;
using ON.Authentication;
using ON.Fragments.Authentication;
using ON.Fragments.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SCNRWeb.Models.CMS;
using ON.Settings;
using ON.Fragments.Settings;

namespace SCNRWeb.Services
{
    public class ContentService
    {
        private readonly ServiceNameHelper nameHelper;
        private readonly SettingsService settingsService;
        public readonly ONUser User;

        public ContentService(ServiceNameHelper nameHelper, SettingsService settingsService, ONUserHelper userHelper)
        {
            User = userHelper.MyUser;

            this.nameHelper = nameHelper;
            this.settingsService = settingsService;
        }

        public async Task<ContentRecord> CreateContent(NewVideoViewModel vm)
        {
            if (!User.CanCreateContent)
                return null;

            var req = new CreateContentRequest
            {
                Public = new()
                {
                    Title = vm.Title,
                    Description = vm.Subtitle ?? "",
                    Author = vm.Author ?? "",
                    SubscriptionLevel = vm.Level,

                    Video = new()
                    {
                        RumbleVideoId = vm.RumbleVideoId ?? "",
                        YoutubeVideoId = vm.YoutubeVideoId ?? "",
                        HtmlBody = vm.Body ?? ""
                    }
                },
                Private = new()
                {
                    Video = new()
                    {
                    },
                }
            };

            var client = new ContentInterface.ContentInterfaceClient(nameHelper.ContentServiceChannel);
            var res = await client.CreateContentAsync(req, GetMetadata());

            return res?.Record;
        }

        public async Task<ContentRecord> CreateContent(NewWrittenViewModel vm)
        {
            if (!User.CanCreateContent)
                return null;

            var req = new CreateContentRequest
            {
                Public = new()
                {
                    Title = vm.Title,
                    Description = vm.Subtitle ?? "",
                    Author = vm.Author ?? "",
                    SubscriptionLevel = vm.Level,

                    Written = new()
                    {
                        HtmlBody = vm.Body ?? ""
                    }
                },
                Private = new()
                {
                    Written = new()
                    {
                    },
                }
            };

            req.Public.CategoryIds.Add(vm.CategoryID ?? "");
            req.Public.Tags.AddRange((vm.Tags ?? "").ToLower().Split(',').Distinct().Where(s => !string.IsNullOrWhiteSpace(s)));

            var client = new ContentInterface.ContentInterfaceClient(nameHelper.ContentServiceChannel);
            var res = await client.CreateContentAsync(req, GetMetadata());

            return res?.Record;
        }

        public async Task DeleteContent(Guid contentId)
        {
            if (!User.CanPublish)
                return;

            var res = await GetContentAdmin(contentId);
            if (res == null)
                return;

            var req = new DeleteContentRequest()
            {
                ContentID = contentId.ToString(),
            };

            var client = new ContentInterface.ContentInterfaceClient(nameHelper.ContentServiceChannel);
            await client.DeleteContentAsync(req, GetMetadata());
        }

        public async Task<GetAllContentResponse> GetAll(GetAllContentRequest request)
        {
            var client = new ContentInterface.ContentInterfaceClient(nameHelper.ContentServiceChannel);
            var res = await client.GetAllContentAsync(request, GetMetadata());

            return res;
        }

        public async Task<GetAllContentAdminResponse> GetAllAdmin(GetAllContentAdminRequest request)
        {
            var client = new ContentInterface.ContentInterfaceClient(nameHelper.ContentServiceChannel);
            var res = await client.GetAllContentAdminAsync(request, GetMetadata());

            return res;
        }

        public async Task<ContentPublicRecord> GetContent(Guid contentId)
        {
            var req = new GetContentRequest
            {
                ContentID = contentId.ToString(),
            };

            var client = new ContentInterface.ContentInterfaceClient(nameHelper.ContentServiceChannel);
            var res = await client.GetContentAsync(req, GetMetadata());

            return res?.Record;
        }

        public async Task<ContentRecord> GetContentAdmin(Guid contentId)
        {
            var req = new GetContentAdminRequest
            {
                ContentID = contentId.ToString(),
            };

            var client = new ContentInterface.ContentInterfaceClient(nameHelper.ContentServiceChannel);
            var res = await client.GetContentAdminAsync(req, GetMetadata());

            return res?.Record;
        }

        public async Task<List<CategoryRecord>> GetRecentCategories()
        {
            var client = new ContentInterface.ContentInterfaceClient(nameHelper.ContentServiceChannel);
            var res = await client.GetRecentCategoriesAsync(new() { NumCategories = 10}, GetMetadata());

            if (res == null)
                return new();

            return await settingsService.GetCategoriesByIds(res.CategoryIds.ToArray());
        }

        public async Task<IEnumerable<string>> GetRecentTags()
        {
            var client = new ContentInterface.ContentInterfaceClient(nameHelper.ContentServiceChannel);
            var res = await client.GetRecentTagsAsync(new() { NumTags = 10 }, GetMetadata());

            return res?.Tags?.ToList() ?? Enumerable.Empty<string>();
        }

        public async Task PublishContent(Guid contentId, DateTime publishOnUTC)
        {
            if (!User.CanPublish)
                return;

            var res = await GetContentAdmin(contentId);
            if (res == null)
                return;

            var req = new PublishContentRequest()
            {
                ContentID = contentId.ToString(),
                PublishOnUTC = Timestamp.FromDateTimeOffset(publishOnUTC),
            };

            var client = new ContentInterface.ContentInterfaceClient(nameHelper.ContentServiceChannel);
            await client.PublishContentAsync(req, GetMetadata());
        }

        public async Task<SearchContentResponse> Search(SearchContentRequest request)
        {
            var client = new ContentInterface.ContentInterfaceClient(nameHelper.ContentServiceChannel);
            var res = await client.SearchContentAsync(request, GetMetadata());

            return res;
        }

        public async Task UndeleteContent(Guid contentId)
        {
            if (!User.CanPublish)
                return;

            var res = await GetContentAdmin(contentId);
            if (res == null)
                return;

            var req = new UndeleteContentRequest()
            {
                ContentID = contentId.ToString(),
            };

            var client = new ContentInterface.ContentInterfaceClient(nameHelper.ContentServiceChannel);
            await client.UndeleteContentAsync(req, GetMetadata());
        }

        public async Task UnpublishContent(Guid contentId)
        {
            if (!User.CanPublish)
                return;

            var res = await GetContentAdmin(contentId);
            if (res == null)
                return;

            var req = new UnpublishContentRequest()
            {
                ContentID = contentId.ToString(),
            };

            var client = new ContentInterface.ContentInterfaceClient(nameHelper.ContentServiceChannel);
            await client.UnpublishContentAsync(req, GetMetadata());
        }

        public async Task<ContentRecord> UpdateContent(Guid contentId, EditVideoViewModel vm)
        {
            if (!User.CanCreateContent)
                return null;

            var client = new ContentInterface.ContentInterfaceClient(nameHelper.ContentServiceChannel);
            var record = await GetContentAdmin(contentId);

            record.Public.Data.Title = vm.Title;
            record.Public.Data.Description = vm.Subtitle ?? "";
            record.Public.Data.Author = vm.Author ?? "";
            record.Public.Data.SubscriptionLevel = vm.Level;
            record.Public.Data.Video.RumbleVideoId = vm.RumbleVideoId ?? "";
            record.Public.Data.Video.YoutubeVideoId = vm.YoutubeVideoId ?? "";
            record.Public.Data.Video.IsLiveStream = vm.IsLiveStream;
            record.Public.Data.Video.IsLive = vm.IsLive;
            record.Public.Data.Video.HtmlBody = vm.Body ?? "";
            record.Public.Data.FeaturedImageAssetID = vm.FeaturedImageAssetID ?? "";

            record.Public.Data.ChannelIds.Clear();
            record.Public.Data.ChannelIds.Add(vm.ChannelID ?? "");

            var req = new ModifyContentRequest()
            {
                ContentID = record.Public.ContentID,
                Public = record.Public.Data,
                Private = record.Private.Data,
            };

            var res = await client.ModifyContentAsync(req, GetMetadata());

            return res?.Record;
        }

        public async Task<ContentRecord> UpdateContent(Guid contentId, EditWrittenViewModel vm)
        {
            if (!User.CanCreateContent)
                return null;

            var client = new ContentInterface.ContentInterfaceClient(nameHelper.ContentServiceChannel);
            var record = await GetContentAdmin(contentId);

            record.Public.Data.Title = vm.Title;
            record.Public.Data.Description = vm.Subtitle ?? "";
            record.Public.Data.Author = vm.Author ?? "";
            record.Public.Data.SubscriptionLevel = vm.Level;
            record.Public.Data.Written.HtmlBody = vm.Body ?? "";
            record.Public.Data.FeaturedImageAssetID = vm.FeaturedImageAssetID ?? "";

            record.Public.Data.CategoryIds.Clear();
            record.Public.Data.CategoryIds.Add(vm.CategoryID ?? "");

            record.Public.Data.Tags.Clear();
            record.Public.Data.Tags.AddRange((vm.Tags ?? "").ToLower().Split(',').Distinct().Where(s => !string.IsNullOrWhiteSpace(s)));

            var req = new ModifyContentRequest()
            {
                ContentID = record.Public.ContentID,
                Public = record.Public.Data,
                Private = record.Private.Data,
            };

            var res = await client.ModifyContentAsync(req, GetMetadata());

            return res?.Record;
        }

        public async Task<ContentRecord> UpdateFeaturedItem(Guid contentId, Guid assetId)
        {
            if (!User.CanCreateContent)
                return null;

            var client = new ContentInterface.ContentInterfaceClient(nameHelper.ContentServiceChannel);
            var record = await GetContentAdmin(contentId);

            record.Public.Data.FeaturedImageAssetID = assetId.ToString();

            var req = new ModifyContentRequest()
            {
                ContentID = record.Public.ContentID,
                Public = record.Public.Data,
                Private = record.Private.Data,
            };

            var res = await client.ModifyContentAsync(req, GetMetadata());

            return res?.Record;
        }

        private Metadata GetMetadata()
        {
            var data = new Metadata();
            if (User != null && !string.IsNullOrWhiteSpace(User.JwtToken))
                data.Add("Authorization", "Bearer " + User.JwtToken);

            return data;
        }
    }
}
