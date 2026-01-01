using Microsoft.AspNetCore.Http;
using SimpleAuthApi;
using System.IO;
using System.Text;
using System.Threading.Tasks;

public class EncryptedResponseMiddleware
{
    private readonly RequestDelegate _next;

    public EncryptedResponseMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var originalBody = context.Response.Body;

        using var memStream = new MemoryStream();
        context.Response.Body = memStream;

        await _next(context); // Controller executes

        memStream.Seek(0, SeekOrigin.Begin);
        string responseBody = await new StreamReader(memStream).ReadToEndAsync();

        // 🔐 Encrypt response
        string encryptedResponse = AesEncryptionHelper.Encrypt(responseBody);

        var encryptedBytes = Encoding.UTF8.GetBytes(encryptedResponse);

        context.Response.Body = originalBody;
        context.Response.ContentLength = encryptedBytes.Length;
        context.Response.ContentType = "text/plain";

        await context.Response.Body.WriteAsync(encryptedBytes);
    }
}
