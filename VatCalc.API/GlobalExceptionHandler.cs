using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using static VatCalc.API.Features.VatCalc.GetVat;

namespace VatCalc.API;
public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler {
	public async ValueTask<bool> TryHandleAsync(
		HttpContext httpContext,
		Exception exception,
		CancellationToken cancellationToken) {
		logger.LogError(
			exception, "Exception occurred: {Message} at {Time}", exception.Message, DateTime.UtcNow);

		(string Detail, string Title, int StatusCode) details = exception switch {
			InvalidAmountException 
			or MoreThanOneInputException 
			or InvalidVatRateException 
			or BadHttpRequestException => (
				exception.Message,
				exception.GetType().Name,
				StatusCodes.Status400BadRequest
			),
			_ => (
				exception.Message,
				exception.GetType().Name,
				StatusCodes.Status500InternalServerError
			)
		};
		httpContext.Response.StatusCode = details.StatusCode;
		var problemDetails = new ProblemDetails {
			Title = details.Title,
			Detail = details.Detail,
			Status = details.StatusCode,
			Instance = httpContext.Request.Path
		};
		problemDetails.Extensions.Add("traceId", httpContext.TraceIdentifier);
		await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
		return true;
	}
}
