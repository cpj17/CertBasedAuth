using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;

namespace WebApp
{
    public partial class Login : System.Web.UI.Page
    {
        //protected void btnLogin_Click(object sender, EventArgs e)
        //{
        //    // Create request object
        //    clsRequest request = new clsRequest
        //    {
        //        Username = txtUsername.Text,
        //        Password = txtPassword.Text
        //    };
        //    string str = Server.MapPath("~/client.pfx");
        //    var handler = new HttpClientHandler();

        //    // Load client certificate
        //    X509Certificate2 cert = new X509Certificate2(
        //        Server.MapPath("~/client.pfx"),
        //        "123");

        //    handler.ClientCertificates.Add(cert);

        //    // API URL
        //    string apiUrl = "https://localhost:7124/api/Auth/login";

        //    // Convert object to JSON
        //    string jsonData = JsonSerializer.Serialize(request);
        //    StringContent content = new StringContent(
        //        jsonData, Encoding.UTF8, "application/json");

        //    // Call API
        //    using (HttpClient client = new HttpClient(handler))
        //    {
        //        try
        //        {
        //            HttpResponseMessage response =
        //                client.PostAsync(apiUrl, content).Result;

        //            if (response.IsSuccessStatusCode)
        //            {
        //                string apiResponse =
        //                    response.Content.ReadAsStringAsync().Result;

        //                lblResult.Text = "API Response: " + apiResponse;
        //            }
        //            else
        //            {
        //                lblResult.Text = "Login failed";
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            lblResult.Text = ex.Message;
        //        }
        //    }
        //}

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            clsRequest request = new clsRequest
            {
                Username = txtUsername.Text,
                Password = txtPassword.Text
            };

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            var handler = new HttpClientHandler();

            // 1️⃣ Load client certificate
            X509Certificate2 cert = new X509Certificate2(
                Server.MapPath("~/client.pfx"),
                "123");
            
            handler.ClientCertificates.Add(cert);

            using (HttpClient client = new HttpClient(handler))
            {
                var json = System.Text.Json.JsonSerializer.Serialize(request);
                var content = new StringContent(
                    json,
                    Encoding.UTF8,
                    "application/json");

                var response = client.PostAsync(
                    "https://localhost:7124/api/Auth/login",
                    content).Result;

                string encryptedResponse = response.Content.ReadAsStringAsync().Result;

                string decryptedResponse = Decrypt(encryptedResponse);

                lblResult.Text = decryptedResponse;
            }
        }

        public static string Decrypt(string cipherText)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes("1234567890123456");
                aes.IV = Encoding.UTF8.GetBytes("6543210987654321");

                byte[] buffer = Convert.FromBase64String(cipherText);

                using (ICryptoTransform decryptor = aes.CreateDecryptor())
                using (MemoryStream ms = new MemoryStream(buffer))
                using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                using (StreamReader sr = new StreamReader(cs))
                {
                    return sr.ReadToEnd();
                }
            }
        }



    }

    public class clsRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
