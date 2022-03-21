using Cesxhin.AnimeSaturn.Domain.DTO;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Cesxhin.AnimeSaturn.Update
{
    using Microsoft.Extensions.Hosting;
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
        }
    }
}
