namespace ModdableWebServer.Helper.Test;

public class UrlHelperTest
{
    [Test]
    public void FormatLength()
    {
        Dictionary<string, string> expected = [];
        Dictionary<string, string> kvs = [];

        Assert.That(UrlHelper.Match("/myurlTest", "/myurlTest", out kvs), Is.True);
        Assert.That(expected, Is.EqualTo(kvs));

        expected = new()
        {
            { "meowparam" = "meow" },
        };
        Assert.That(UrlHelper.Match("/myurlTest/meow", "/myurlTest/{meowparam}", out kvs), Is.True);
        Assert.That(expected, Is.EqualTo(kvs));

        expected = new()
        {
            { "arg1" = "1" },
            { "arg2" = "2" },
        };
        Assert.That(UrlHelper.Match("/myurlTest/test?arg1=1&arg2=2", "/myurlTest/test?arg1={arg1}&arg2={arg2}", out kvs), Is.True);
        Assert.That(expected, Is.EqualTo(kvs));

        expected = new()
        {
            { "arg1" = "1" },
            { "arg2" = "2" },
        };
        Assert.That(UrlHelper.Match("/myurlTest/test?arg1=1&arg2=2", "/myurlTest/test?{!args}", out kvs), Is.True);
        Assert.That(expected, Is.EqualTo(kvs));
    }
}