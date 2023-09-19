using Grpc.Core;
using ON.Authentication;
using ON.Fragments.Content;
using System.Threading.Tasks;
using System;
using ON.Settings;
using Microsoft.AspNetCore.Http;
using Google.Protobuf;

namespace SubverseWeb.Services
{
    public class AssetService
    {
        private readonly ServiceNameHelper nameHelper;
        public readonly ONUser User;

        public AssetService(ServiceNameHelper nameHelper, ONUserHelper userHelper)
        {
            User = userHelper.MyUser;

            this.nameHelper = nameHelper;
        }

        public async Task<CreateAssetResponse> CreateImage(IFormFile file, string title = "", string caption = "")
        {
            using var stream = file.OpenReadStream();

            var req = new CreateAssetRequest
            {
                Image = new()
                {
                    Public = new()
                    {
                        Title = title,
                        Caption = caption,
                        MimeType = file.ContentType,
                        Data = await ByteString.FromStreamAsync(stream),
                    },
                    Private = new()
                    {
                        
                    }
                }
            };

            var client = new AssetInterface.AssetInterfaceClient(nameHelper.ContentServiceChannel);
            var res = await client.CreateAssetAsync(req, GetMetadata());

            return res;
        }

        public async Task<ImageAssetPublicRecord> GetImage(Guid contentId)
        {
            var req = new GetAssetRequest
            {
                AssetID = contentId.ToString(),
            };

            var client = new AssetInterface.AssetInterfaceClient(nameHelper.ContentServiceChannel);
            var res = await client.GetAssetAsync(req, GetMetadata());

            return res?.Image;
        }

        public async Task<SearchAssetResponse> SearchImages(SearchAssetRequest req)
        {
            var client = new AssetInterface.AssetInterfaceClient(nameHelper.ContentServiceChannel);
            var res = await client.SearchAssetAsync(req, GetMetadata());

            return res;
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
