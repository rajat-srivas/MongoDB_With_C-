using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Bson.IO;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;


namespace MongoDB.Operations.Middleware
{
	public class ExceptionResponse
	{
		private HttpStatusCode _code;
		private string _msg;

		public ExceptionResponse(HttpStatusCode code, string msg)
		{
			this._code = code;
			this._msg = msg;
		}

		public HttpStatusCode StatusCode { get; set; }
		public string Description { get; set; }
	}

	public class GlobalExceptionHandlerMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

        public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> log)
        {
            _next = next;
            _logger = log;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch(Exception ex)
			{
				await HandleExceptionAsync(context, ex);
			}
		}

		private async Task HandleExceptionAsync(HttpContext context, Exception ex)
		{
			_logger.LogError(ex.Message);
			var problemDetails = new ProblemDetails
			{
				Status = StatusCodes.Status500InternalServerError,
				Title = "Server Error"
			};

			problemDetails.Title = ex switch
			{
				ArgumentException _ => "Some data missing",
				ApplicationException _ => "Bad Request",
				_ => "Something went wrong"
			};

			context.Response.StatusCode =
				StatusCodes.Status500InternalServerError;

			context.Response.ContentType = "application/json";
			await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails));
		}
	}
}
