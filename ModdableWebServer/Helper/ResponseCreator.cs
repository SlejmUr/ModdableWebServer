using NetCoreServer;

namespace ModdableWebServer.Helper
{
    public class ResponseCreator
    {
        private HttpResponse response;
        public ResponseCreator(int status = 200)
        {
            response = new();
            New(status);
        }

        public ResponseCreator(int status, Dictionary<string, string> Headers, string content)
        {
            response = new();
            New(status);
            SetHeaders(Headers);
            SetBody(content);
        }

        public ResponseCreator(int status, Dictionary<string, string> Headers, byte[] content)
        {
            response = new();
            New(status);
            SetHeaders(Headers);
            SetBody(content);
        }

        public void New(int status = 200)
        {
            response.Clear();
            response.SetBegin(status);
        }

        public void SetHeaders(Dictionary<string, string> Headers)
        {
            foreach (var Header in Headers)
            {
                SetHeader(Header.Key, Header.Value);
            }
        }

        public void SetHeader(string key, string value)
        {
            response.SetHeader(key, value);
        }

        public void SetCookie(string name, string value, int maxAge = 86400000, string path = "", string domain = "", bool secure = true, bool strict = false, bool httpOnly = true)
        {
            response.SetCookie(name, value, maxAge, path, domain, secure, strict, httpOnly);
        }

        public void SetBody(string content)
        {
            response.SetBody(content);
        }

        public void SetBody(byte[] content)
        {
            response.SetBody(content);
        }

        public void SetResponse(HttpResponse rsp)
        {
            response = rsp;
        }

        public HttpResponse GetResponse()
        {
            return response;
        }

    }
}
