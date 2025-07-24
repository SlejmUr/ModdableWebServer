namespace ModdableWebServer.Helper.Test;

public class UrlHelperTest
{
    [Test]
    public void TestURLRegular()
    {
        Dictionary<string, string> expected = [];
        Dictionary<string, string> kvs = [];

        bool ret = UrlHelper.Match("/myurlTest", "/myurlTest", out kvs);
        Assert.That(UrlHelper.ReasonFail, Is.Empty);
        Assert.That(ret, Is.True);
        Assert.That(expected, Is.EqualTo(kvs));
    }

    [Test]
    public void TestURLWithParam()
    {
        Dictionary<string, string> expected = new()
        {
            { "meowparam", "meow" },
        };
        Dictionary<string, string> kvs = [];

        bool ret = UrlHelper.Match("/myurlTest/meow", "/myurlTest/{meowparam}", out kvs);
        Assert.That(UrlHelper.ReasonFail, Is.Empty);
        Assert.That(ret, Is.True);
        Assert.That(expected, Is.EqualTo(kvs));
    }

    [Test]
    public void TestURLWithArg1()
    {
        Dictionary<string, string> expected = new()
        {
            { "arg1" , "test" },
        };
        Dictionary<string, string> kvs = [];

        bool ret = UrlHelper.Match("/myurlTest/test?arg1=test", "/myurlTest/test?arg1={arg1}", out kvs);
        Assert.That(UrlHelper.ReasonFail, Is.Empty);
        Assert.That(ret, Is.True);
        Assert.That(expected, Is.EqualTo(kvs));
    }

    [Test]
    public void TestURLWithArg2()
    {
        Dictionary<string, string> expected = new()
        {
            { "arg1" , "1" },
            { "arg2" , "2" },
        };
        Dictionary<string, string> kvs = [];

        bool ret = UrlHelper.Match("/myurlTest/test?arg1=1&arg2=2", "/myurlTest/test?arg1={arg1}&arg2={arg2}", out kvs);
        Assert.That(UrlHelper.ReasonFail, Is.Empty);
        Assert.That(ret, Is.True);
        Assert.That(expected, Is.EqualTo(kvs));
    }


    [Test]
    public void TestURLWithALLArg()
    {
        Dictionary<string, string> expected = new()
        {
            { "arg1" , "1" },
            { "arg2" , "2" },
        };
        Dictionary<string, string> kvs = [];

        bool ret = UrlHelper.Match("/myurlTest/test?arg1=1&arg2=2", "/myurlTest/test?{!args}", out kvs);
        Assert.That(UrlHelper.ReasonFail, Is.Empty);
        Assert.That(ret, Is.True);
        Assert.That(expected, Is.EqualTo(kvs));
    }
}