using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using System.Threading;
using System.Text.Json;
using Microsoft.AspNetCore.Http.HttpResults;

namespace MongoDB.Operations.Middleware
{
	public class GlobalExceptionHandler : IExceptionHandler
	{
		public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
		{
			var problemDetails = new ProblemDetails();

			problemDetails = exception switch
			{
				ArgumentNullException => new ProblemDetails
				{
					Status = StatusCodes.Status400BadRequest,
					Title = exception.Message
				},
				ApplicationException => new ProblemDetails
				{
					Status = StatusCodes.Status500InternalServerError,
					Title = exception.Message
				},
				_ => new ProblemDetails
				{
					Status = StatusCodes.Status500InternalServerError,
					Title = exception.Message
				}
			};

			httpContext.Response.ContentType = "application/json";
			httpContext.Response.StatusCode = problemDetails.Status.Value;

			await httpContext.Response
				.WriteAsync(JsonSerializer.Serialize(problemDetails));

			return true;
		}
	}
}
