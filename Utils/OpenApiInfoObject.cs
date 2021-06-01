using System;
using System.Collections.Generic;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.OpenApi.Models;

namespace CoWinAlert.Function
{
    public class OpenApiConfigurationOptions : IOpenApiConfigurationOptions
    {
        public OpenApiInfo Info { get; set; } = new OpenApiInfo()
        {
            Version = "2.0.0",
            Title = "CoWin Alert",
            Description = "Register Yourself Here.\n Add your details and get notified whenever a vaccination slot opens up!.\n **Stay Ahead of the Curve.**",
            Contact = new OpenApiContact()
            {
                Name = "Captain Nemo",
                Email = "captain.nemo.github@gmail.com",
                Url = new Uri("https://www.instagram.com/captain._nemo/")
            },
            License = new OpenApiLicense()
            {
                Name = "LinkedIn",
                Url = new Uri("https://www.linkedin.com/in/rohitashwachaks/"),
            }
        };

        public List<OpenApiServer> Servers { get; set; } = new List<OpenApiServer>();

        public OpenApiVersionType OpenApiVersion { get; set; } = OpenApiVersionType.V2;
        // List<OpenApiServer> IOpenApiConfigurationOptions.Servers { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        // OpenApiVersionType IOpenApiConfigurationOptions.OpenApiVersion { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}