using ON.Authentication;
using ON.Fragments.Content;
using System.Collections.Generic;

namespace SubverseWeb.Models.CMS
{
    public class PickImageViewModel
    {
        public PickImageViewModel() { }

        public PickImageViewModel(SearchAssetResponse assetResponse)
        {
            if (assetResponse?.Records == null)
                return;
            Records.AddRange(assetResponse.Records);
        }

        public string Id { get; set; }
        public List<AssetListRecord> Records { get; } = new List<AssetListRecord>();
        public PageNumViewModel PageVM { get; set; } = null;
    }
}
