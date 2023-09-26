namespace SCNRWeb.Models.SiteSettings
{
    public class SendgridOwnerPostViewModel
    {
        public bool Enabled { get; set; }
        public string ApiKeySecret { get; set; }
        public string SendFromAddress { get; set; }
    }
}
