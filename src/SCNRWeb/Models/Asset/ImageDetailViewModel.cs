using ON.Fragments.Content;

namespace SubverseWeb.Models.Asset
{
    public class ImageDetailViewModel
    {
        public ImageDetailViewModel() { }
        public ImageDetailViewModel(ImageAssetPublicRecord image)
        {
            Id = image.AssetID;
            Title = image.Data.Title;
            Caption = image.Data.Caption;
        }

        public string Id { get; set; }
        public string Title { get; set; }
        public string Caption { get; set; }
    }
}
