using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using ON.Fragments.Content;
using System;

namespace SCNRWeb.Helper
{
    public class ContentUrlHelper
    {
        private readonly string host;
        public ContentUrlHelper(IHttpContextAccessor accessor)
        {
            var req = accessor.HttpContext.Request;
            host = $"{req.Scheme}://{req.Host}";
        }

        public string GenerateFullArticleUrl(ContentListRecord rec) => host + GeneratePartialArticleUrl(rec);
        public string GenerateFullArticleUrl(ContentPublicRecord rec) => host + GeneratePartialArticleUrl(rec);
        public string GenerateFullArticleUrl(Guid id, string stub) => host + GeneratePartialArticleUrl(id, stub);
        public string GenerateFullImageUrl(ContentListRecord rec) => host + GeneratePartialImageUrl(rec);
        public string GenerateFullImageUrl(ContentPublicRecord rec) => host + GeneratePartialImageUrl(rec);
        public string GenerateFullImageUrl(Guid id) => host + GeneratePartialImageUrl(id);

        public string GeneratePartialArticleUrl(ContentListRecord rec) => rec == null ? "" : GeneratePartialArticleUrl(rec.ContentIDGuid, rec.URL);
        public string GeneratePartialArticleUrl(ContentPublicRecord rec) => rec == null ? "" : GeneratePartialArticleUrl(rec.ContentIDGuid, rec.Data.URL);
        public string GeneratePartialArticleUrl(Guid id, string stub) => $"/article/{stub}_{ConvertGuid(id)}";
        public string GeneratePartialImageUrl(ContentListRecord rec) => GeneratePartialImageUrl(rec?.FeaturedImageAssetID);
        public string GeneratePartialImageUrl(ContentPublicRecord rec) => GeneratePartialImageUrl(rec?.Data?.FeaturedImageAssetID);

        public string GeneratePartialImageUrl(string str)
        {
            if (str == null) return "";
            Guid id;
            if (!Guid.TryParse(str, out id))
                return "";

            return GeneratePartialImageUrl(id);
        }

        public string GeneratePartialImageUrl(Guid id) => $"/image/{ConvertGuid(id)}";

        private string ConvertGuid(Guid id) => id.ToString().Replace("-", "");
    }
}
