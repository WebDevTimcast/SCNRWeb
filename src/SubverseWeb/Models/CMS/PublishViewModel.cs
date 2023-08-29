using System;

namespace SubverseWeb.Models.CMS
{
    public class PublishViewModel
    {
        public string ID { get; set; }
        public string ReturnUrl { get; set; }
        public DateTime PublishOnEST { get; set; }
        public string Title { get; set; }

        public string ErrorMessage { get; set; }
    }
}
