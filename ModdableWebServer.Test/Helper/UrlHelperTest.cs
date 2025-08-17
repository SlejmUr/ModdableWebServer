using ModdableWebServer.Helper;

namespace ModdableWebServer.Test.Helper;

public class UrlHelperTest
{
    private static readonly Dictionary<string, string> Empty = [];
    [Test]
    public void TestURLRegular()
    {
        bool ret = UrlHelper.Match("/myurlTest", "/myurlTest", out Dictionary<string, string> kvs);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(UrlHelper.ReasonFail, Is.Empty);
            Assert.That(ret, Is.True);
            Assert.That(Empty, Is.EqualTo(kvs));
        }
    }

    [Test]
    public void TestURLWithParam()
    {
        Dictionary<string, string> expected = new()
        {
            { "meowparam", "meow" },
        };
        bool ret = UrlHelper.Match("/myurlTest/meow", "/myurlTest/{meowparam}", out Dictionary<string, string> kvs);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(UrlHelper.ReasonFail, Is.Empty);
            Assert.That(ret, Is.True);
            Assert.That(expected, Is.EqualTo(kvs));
        }
    }

    [Test]
    public void TestURLWithArg1()
    {
        Dictionary<string, string> expected = new()
        {
            { "arg1" , "test" },
        };
        bool ret = UrlHelper.Match("/myurlTest/test?arg1=test", "/myurlTest/test?arg1={arg1}", out Dictionary<string, string> kvs);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(UrlHelper.ReasonFail, Is.Empty);
            Assert.That(ret, Is.True);
            Assert.That(expected, Is.EqualTo(kvs));
        }
    }

    [Test]
    public void TestURLWithArg2()
    {
        Dictionary<string, string> expected = new()
        {
            { "arg1" , "1" },
            { "arg2" , "2" },
        };

        bool ret = UrlHelper.Match("/myurlTest/test?arg1=1&arg2=2", "/myurlTest/test?arg1={arg1}&arg2={arg2}", out Dictionary<string, string> kvs);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(UrlHelper.ReasonFail, Is.Empty);
            Assert.That(ret, Is.True);
            Assert.That(expected, Is.EqualTo(kvs));
        }
    }


    [Test]
    public void TestURLWithALLArg()
    {
        Dictionary<string, string> expected = new()
        {
            { "arg1" , "1" },
            { "arg2" , "2" },
        };

        bool ret = UrlHelper.Match("/myurlTest/test?arg1=1&arg2=2", "/myurlTest/test?{!args}", out Dictionary<string, string> kvs);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(UrlHelper.ReasonFail, Is.Empty);
            Assert.That(ret, Is.True);
            Assert.That(expected, Is.EqualTo(kvs));
        }
    }

    [Test]
    public void TestEveryArg()
    {
        Dictionary<string, string> expected = new()
        {
            { "param" , "myurlTest" },
            { "yeet" , "test" },
            { "arg1" , "1" },
            { "arg2" , "2" },
        };

        bool ret = UrlHelper.Match("/myurlTest/test?arg1=1&arg2=2", "/{param}/{yeet}/?{!args}", out Dictionary<string, string> kvs);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(UrlHelper.ReasonFail, Is.Empty);
            Assert.That(ret, Is.True);
            Assert.That(expected, Is.EqualTo(kvs));
        }
    }
}