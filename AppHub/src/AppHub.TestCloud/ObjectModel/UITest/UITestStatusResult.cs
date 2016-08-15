namespace Microsoft.AppHub.TestCloud
{
    public class StatusResult
    {
        public string Status { get; set; }

        public string StatusDescription { get; set; }

        public string NUnitReportsUrl { get; set; }

        public string ShareLink { get; set; }

        public UITestData UITestData { get; set; }
    }
}