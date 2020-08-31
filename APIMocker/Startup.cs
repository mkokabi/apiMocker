using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Unicode;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace APIMocker
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public static List<APIDetailOptions> APIDetailOptions { get; set; } = new List<APIDetailOptions>();

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      services.Configure<APIDetailOptions>(
          Configuration.GetSection("APIDetail"));

      services.AddControllers();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      Configuration.GetSection("APIDetail").Bind(APIDetailOptions);

      app.UseStatusCodePages(async context =>
      {
        var request = context.HttpContext.Request;
        var path = request.Path.Value;
        var queryString = request.QueryString.Value;
        var method = request.Method;
        var response = context.HttpContext.Response;
        var match = APIDetailOptions.FirstOrDefault(opt =>
                  path.Equals(opt.Path, StringComparison.OrdinalIgnoreCase) &&
                  QueryStringChecker.Match(opt.QueryString, queryString) && 
                  string.Equals(opt.Method, method, StringComparison.InvariantCultureIgnoreCase));
        if (match != null)
        {
          response.StatusCode = match.StatusCode;
          var stream = response.Body;
          await stream.WriteAsync(Encoding.UTF8.GetBytes(match.ResponseBody));
        }
      });

      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      app.UseHttpsRedirection();

      app.UseRouting();

      app.UseAuthorization();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
        endpoints.MapGet("/", async context => {
          await WriteWelcomePage(context.Response, context.Request);
        });
      });
    }

    private async Task WriteWelcomePage(HttpResponse response, HttpRequest request)
    {
      response.ContentType = "text/html";
      var sb = new StringBuilder("<h2>MockApi</h2>");
      sb.Append("<p style='color: green'>running</p>");
      sb.Append("<br><p>registered API mocks:</p>");
      sb.Append("<style>table {border-collapse: collapse;} td, th { border: 1px solid black; }</style>");
      sb.Append("<table><tr><th>Path</th><th>Query string</th><th>method</th><th></th></tr>");
      APIDetailOptions.ForEach(opt => 
        sb.Append($@"<tr><td>{opt.Path}</td><td>{opt.QueryString}</td><td>{opt.Method.ToUpper()}</td>
        <td><a target='_blank' href='{
          (request.IsHttps ? "https" : "http")}://{request.Host}{opt.Path}{opt.QueryString}'>test</a></td></tr>"));
      sb.Append("</table>");
      await response.BodyWriter.WriteAsync(Encoding.UTF8.GetBytes(sb.ToString()));
    }
  }
}
