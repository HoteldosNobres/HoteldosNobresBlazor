using HoteldosNobresBlazor.Funcoes;
using HoteldosNobresBlazor.Services;
using System.Text;

namespace HoteldosNobresBlazor;
 
public static class SistemaExtensions
{
    public static void AddEndPointsSisTema(this WebApplication app)
    {
        AppState sCOPP = app.Services.GetRequiredService<AppState>();
        var cache = new CacheHotel(sCOPP);
        cache.CacheExecutanado();

        #region Endpoint Whatsapp
        app.MapPost("/whatsapp", async (HttpContext httpContext) =>
        {
            try
            {
                using var reader = new StreamReader(httpContext.Request.Body, Encoding.UTF8);
                string body = await reader.ReadToEndAsync();

                //string verify_token = httpContext.Request.Query["hub.verify_token"];
                //string challenge = httpContext.Request.Query["hub.challenge"];
                //string mode = httpContext.Request.Query["hub.mode"];

                sCOPP.MyMessageLogWhatsapp = " -Bodi-  " + body + " -Bodi-  ";
                CacheHotel cache2 = new CacheHotel(sCOPP);
                cache2.RecebeMensagem(body);

                //sCOPP.MyMessage += verify_token + "<verify_token ";
                //sCOPP.MyMessage += challenge + "<challenge ";
                //sCOPP.MyMessage += mode + "<mode ";

                //if (string.IsNullOrEmpty(verify_token))
                //{
                //    httpContext.Response.StatusCode = 400; // Bad Request
                //    await httpContext.Response.WriteAsync("Token não fornecido.");
                //    return;
                //}
                //bool isValid = verify_token.Replace("Bearer ", "") == "TOKENHOTELDOSNOBRES19";
                //if (!isValid)
                //{
                //    httpContext.Response.StatusCode = 401; // Unauthorized
                //    await httpContext.Response.WriteAsync("Token inválido.");
                //    return;
                //}

                //sCOPP.MyMessage += token + " ";

                httpContext.Response.StatusCode = 200;
                //await httpContext.Response.WriteAsync(challenge);

            }
            catch (Exception ex)
            {
                await httpContext.Response.WriteAsync(ex.Message);
                httpContext.Response.StatusCode = 500;
            }

        });

        app.MapGet("/whatsapp", async (HttpContext httpContext) =>
        {
            try
            {
                //string token = httpContext.Request.Headers["Authorization"];
                ////if (string.IsNullOrEmpty(token))
                ////{
                ////    httpContext.Response.StatusCode = 400; // Bad Request
                ////    await httpContext.Response.WriteAsync("Token não fornecido.");
                ////    return;
                ////}

                ////bool isValid = token.Replace("Bearer ", "") == "TOKENHOTELDOSNOBRES19";
                ////if (!isValid)
                ////{
                ////    httpContext.Response.StatusCode = 401; // Unauthorized
                ////    await httpContext.Response.WriteAsync("Token inválido.");
                ////    return;
                ////}
                ///


                using var reader = new StreamReader(httpContext.Request.Body, Encoding.UTF8);
                string body = await reader.ReadToEndAsync();

                string verify_token = httpContext.Request.Query["hub.verify_token"];
                string challenge = httpContext.Request.Query["hub.challenge"];
                string mode = httpContext.Request.Query["hub.mode"];

                sCOPP.MyMessage += body + " ";
                sCOPP.MyMessage += verify_token + " ";
                sCOPP.MyMessage += challenge + " ";
                sCOPP.MyMessage += mode + " ";


                if (string.IsNullOrEmpty(verify_token))
                {
                    httpContext.Response.StatusCode = 400; // Bad Request
                    await httpContext.Response.WriteAsync("Token não fornecido.");
                    return;
                }
                bool isValid = verify_token.Replace("Bearer ", "") == "TOKENHOTELDOSNOBRES19";
                if (!isValid)
                {
                    httpContext.Response.StatusCode = 401; // Unauthorized
                    await httpContext.Response.WriteAsync("Token inválido.");
                    return;
                }


                httpContext.Response.StatusCode = 200;
                await httpContext.Response.WriteAsync(challenge);

            }
            catch (Exception ex)
            {
                await httpContext.Response.WriteAsync(ex.Message);
                httpContext.Response.StatusCode = 500;
            }

        });

        #endregion

        app.MapPost("/addreserva", async (HttpContext httpContext) =>
        {
            try
            {
                using var reader = new StreamReader(httpContext.Request.Body, Encoding.UTF8);
                string body = await reader.ReadToEndAsync();

                CacheHotel cache2 = new CacheHotel(sCOPP);
                await httpContext.Response.WriteAsync(cache2.CacheCreateReservationAsync(body).Result);

            }
            catch (Exception ex)
            {
                await httpContext.Response.WriteAsync(ex.Message);
                httpContext.Response.StatusCode = 500;
            }

        });

        app.MapPost("/status_changed", async (HttpContext httpContext) =>
        {
            try
            {
                using var reader = new StreamReader(httpContext.Request.Body, Encoding.UTF8);
                string body = await reader.ReadToEndAsync();

                CacheHotel cache2 = new CacheHotel(sCOPP);
                await httpContext.Response.WriteAsync(cache2.CacheChangedStatus(body));

            }
            catch (Exception ex)
            {
                await httpContext.Response.WriteAsync(ex.Message);
                httpContext.Response.StatusCode = 500;
            }

        });

        app.MapPost("/accommodation_changed", async (HttpContext httpContext) =>
        {
            try
            {
                using var reader = new StreamReader(httpContext.Request.Body, Encoding.UTF8);
                string body = await reader.ReadToEndAsync();

                CacheHotel cache2 = new CacheHotel(sCOPP);
                await httpContext.Response.WriteAsync(cache2.CacheAccommodation_changed(body));

            }
            catch (Exception ex)
            {
                await httpContext.Response.WriteAsync(ex.Message);
                httpContext.Response.StatusCode = 500;
            }

        });

        app.MapPost("/details_changed", async (HttpContext httpContext) =>
        {
            try
            {
                using var reader = new StreamReader(httpContext.Request.Body, Encoding.UTF8);
                string body = await reader.ReadToEndAsync();

                CacheHotel cache2 = new CacheHotel(sCOPP);
                await httpContext.Response.WriteAsync(cache2.CacheDetails_changed(body));

            }
            catch (Exception ex)
            {
                await httpContext.Response.WriteAsync(ex.Message);
                httpContext.Response.StatusCode = 500;
            }

        });

    }
}