namespace Presentation.MinimalApi.Common.net7.StartupConfiguration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public interface IStartupConfiguration
{
    static abstract int Order { get; }
    static abstract void ConfigureServices(IServiceCollection services, IConfiguration configuration);
    static abstract void ConfigureApp(WebApplication app);
}
