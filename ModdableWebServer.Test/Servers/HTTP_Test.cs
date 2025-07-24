using NetCoreServer;
using System.Net;
using System.Reflection;

namespace ModdableWebServer.Servers.Test;

public class HTTP_Test
{
    HTTP_Server server;
    HttpClientEx client;

    [SetUp]
    public void SetUp()
    {
        server = new(IPAddress.Any, 6666);
        client = new HttpClientEx(IPAddress.Any, 6666);
    }

    
    [TearDown]
    public void TearDown()
    {
        server.Dispose();
        client.Dispose();
    }

    [Test]
    public void TestCommon()
    {
        server.OverrideAttributes(Assembly.GetAssembly(typeof(WS_Server))!);
        server.OverrideAttributes(Assembly.GetEntryAssembly()!);
        Assert.That(server.Start(), Is.True);

        var response = client.SendGetRequest("/test").Result;
        Assert.That(response.Status, Is.EqualTo(200));

        response = client.SendDeleteRequest("/del").Result;
        Assert.That(response.Status, Is.EqualTo(200));
        Assert.That(response.Body, Is.EqualTo("balls"));

        Assert.That(server.Stop(), Is.True);
    }
}