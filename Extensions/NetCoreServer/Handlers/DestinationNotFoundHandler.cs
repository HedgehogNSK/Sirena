using NetCoreServer;
using System.Net;

namespace Hedgey.Extensions.NetCoreServer;
public class DestinationNotFoundHandler : IHTTPRequestHandler
{
  public HttpResponse Handle(HttpRequest request)
  {
    var response = new HttpResponse((int)HttpStatusCode.NotFound);
    string html = $@"
        <!DOCTYPE html>
        <html lang='en'>
        <head>
            <meta charset='UTF-8'>
            <meta name='viewport' content='width=device-width, initial-scale=1.0'>
            <title>404 Not Found</title>
            <style>
                body {{
                    font-family: Arial, sans-serif;
                    text-align: center;
                    padding: 50px;
                    background-color: #f4f4f4;
                }}
                h1 {{
                    color: #d9534f;
                }}
                p {{
                    color: #666;
                }}
                .container {{
                    max-width: 600px;
                    margin: 0 auto;
                    background: #fff;
                    padding: 20px;
                    border-radius: 10px;
                    box-shadow: 0px 0px 10px rgba(0, 0, 0, 0.1);
                }}
            </style>
        </head>
        <body>
            <div class='container'>
                <h1>404 Not Found</h1>
                <p>The requested URL <b>{request.Url}</b> was not found on this server.</p>
            </div>
        </body>
        </html>";

    response.SetHeader("Content-Type", "text/html; charset=UTF-8");
    response.SetBody(html);
    return response;
  }
}