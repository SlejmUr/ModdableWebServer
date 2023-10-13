namespace ModdableWebServer.Attributes
{
    public class WSAttribute : Attribute
    {
        public string url;

        public WSAttribute(string url)
        {
            this.url = url;
        }
    }
}
