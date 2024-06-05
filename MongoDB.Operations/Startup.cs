using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Operations.Middleware;
using System.Text;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using System;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using System.Linq;

namespace MongoDB.Operations
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddExceptionHandler<GlobalExceptionHandler>();
			services.AddProblemDetails();

			services.AddApiVersioning(options =>
			{
				options.ReportApiVersions = true;
				options.AssumeDefaultVersionWhenUnspecified = true;
				options.DefaultApiVersion = new ApiVersion(1, 0);
				options.ApiVersionReader = new UrlSegmentApiVersionReader(); // or other versioning schemes
			});

			#region Authentication Schema

			services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			})
				 .AddJwtBearer(options =>
				 {
					 options.TokenValidationParameters = new TokenValidationParameters
					 {
						 ValidateIssuer = true,
						 ValidateAudience = true,
						 ValidateLifetime = true,
						 ValidateIssuerSigningKey = true,
						 ValidIssuer = "yourissuer",
						 ValidAudience = "youraudience",
						 IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your_secret_key"))
					 };
				 })
					.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
					{
						options.LoginPath = "/login";
						options.Cookie.Name = "your_cookie_name";
					})
					//.AddApiKeySupport(options => { }); // Custom API Key support
					;

			// Authorization Policies
		services.AddAuthorization(options =>
		{
			options.AddPolicy("AdminPolicy", policy => policy.RequireRole("admin"));
			options.AddPolicy("UserPolicy", policy => policy.RequireRole("user"));
		});

			#endregion

			services.AddControllers();

			services.AddSwaggerGen(c =>
			{
					c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "My API V1", Version = "v1" });
					c.SwaggerDoc("v2", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "My API V2", Version = "v2" });
			});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseAuthentication();
			app.UseAuthorization();

			// Enable middleware to serve generated Swagger as a JSON endpoint.
			app.UseSwagger();

			//Using the old middleware method
			//app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

			//using the new IExceptionHandler in .Net 8

			app.UseExceptionHandler();

			// Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
			// specifying the Swagger JSON endpoint.
			app.UseSwaggerUI(c =>
			{
				c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
				c.SwaggerEndpoint("/swagger/v2/swagger.json", "My API V2");
				c.RoutePrefix = string.Empty; // To serve the Swagger UI at the app's root (http://localhost:<port>/)
			});

			app.UseHttpsRedirection();

			app.UseRouting();

			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}
	}
}
