using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using static System.Net.WebRequestMethods;

var builder = WebApplication.CreateBuilder(args);

// extra
builder.WebHost.ConfigureKestrel(options =>
{
    options.ConfigureHttpsDefaults(o =>
    {
        o.ClientCertificateMode = ClientCertificateMode.RequireCertificate;
        o.SslProtocols =
            System.Security.Authentication.SslProtocols.Tls12 |
            System.Security.Authentication.SslProtocols.Tls13;
    });
});
builder.Logging.AddConsole();

// extra

// Add certificate authentication
builder.Services.AddAuthentication(
    CertificateAuthenticationDefaults.AuthenticationScheme)
    .AddCertificate(options =>
    {
        options.AllowedCertificateTypes =
            CertificateTypes.SelfSigned;

        options.ValidateCertificateUse = false;
        options.ValidateValidityPeriod = false;

        options.Events = new CertificateAuthenticationEvents
        {
            OnCertificateValidated = context =>
            {
                var cert = context.ClientCertificate;

                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, context.ClientCertificate.Subject)
                };

                context.Principal = new ClaimsPrincipal(
                    new ClaimsIdentity(claims, context.Scheme.Name));

                // Expiry validation
                if (DateTime.UtcNow.AddDays(10) < cert.NotBefore ||
                    DateTime.UtcNow.AddDays(10) > cert.NotAfter)
                {
                    context.Fail("Certificate expired or not yet valid");
                    return Task.CompletedTask;
                }

                // Issuer validation
                if (!cert.Issuer.Contains("CN=MyClientCert"))
                {
                    context.Fail("Untrusted certificate issuer");
                    return Task.CompletedTask;
                }

                // Revocation validation
                if (!ValidateCertificateRevocation(cert))
                {
                    context.Fail("Certificate revoked");
                    return Task.CompletedTask;
                }

                // Validate thumbprint (IMPORTANT)
                if (cert.Thumbprint == "3B01C7D75AD75323F973BDFBE9F6E1874E8D9704")
                {
                    context.Success();
                }
                else
                {
                    context.Fail("Invalid Certificate");
                }

                // All checks passed
                context.Success();
                return Task.CompletedTask;
            }
        };
    });

static bool ValidateCertificateRevocation(X509Certificate2 cert)
{
    using var chain = new X509Chain();

    chain.ChainPolicy = new X509ChainPolicy
    {
        RevocationMode = X509RevocationMode.Online,
        RevocationFlag = X509RevocationFlag.EntireChain,
        VerificationFlags = X509VerificationFlags.NoFlag,
        UrlRetrievalTimeout = TimeSpan.FromSeconds(10)
    };

    bool isValid = chain.Build(cert);

    if (!isValid)
    {
        foreach (var status in chain.ChainStatus)
        {
            if (status.Status == X509ChainStatusFlags.Revoked)
            {
                return false;
            }
        }
    }

    return isValid;
}

// Add certificate authentication


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<EncryptedResponseMiddleware>();

app.MapControllers();

app.UseCertificateForwarding();  // This one for Certificate based authentication

app.Run();
