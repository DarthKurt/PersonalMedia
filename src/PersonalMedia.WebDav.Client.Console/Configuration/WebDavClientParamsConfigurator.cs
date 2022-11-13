using System.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace PersonalMedia.WebDav.Client.Console.Configuration;

internal class WebDavClientParamsConfigurator : IConfigureOptions<WebDavClientParams>
{
    private readonly IConfiguration _configuration;

    public WebDavClientParamsConfigurator(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Configure(WebDavClientParams clientParams)
    {
        var section = _configuration.GetSection("DavServer");
        var url = section.GetValue<Uri>("Url");
        if (url != null)
        {
            clientParams.BaseAddress = url;
        }

        var user = section.GetValue<string>("Username");
        if (string.IsNullOrWhiteSpace(user))
            return;

        var pwd = section.GetValue<string>("Password");
        clientParams.UseDefaultCredentials = false;
        clientParams.Credentials = new NetworkCredential(user, pwd);
    }
}
