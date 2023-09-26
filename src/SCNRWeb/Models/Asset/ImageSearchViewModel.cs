using ON.Authentication;
using ON.Fragments.Content;
using System.Collections.Generic;

namespace SCNRWeb.Models.Asset
{
    public class ImageSearchViewModel
    {
        public ImageSearchViewModel() { }

        public ImageSearchViewModel(SearchAssetResponse assetResponse)
        {
            if (assetResponse?.Records == null)
                return;
            Records.AddRange(assetResponse.Records);
        }

        public List<AssetListRecord> Records { get; } = new List<AssetListRecord>();
        public PageNumViewModel PageVM { get; set; } = null;
    }
}
