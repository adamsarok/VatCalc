﻿using static VatCalc.API.Features.VatCalc.GetVat;

namespace VatCalc.Tests.Features.VatCalc;
public class VatCalcSystemTests(WebApplicationFactory<Program> factory) : VatCalcTestBase, IClassFixture<WebApplicationFactory<Program>> {

	[Fact]
	public async Task GetVatAmounts() {
		var client = factory.CreateClient();
		foreach (var input in SuccessInput) {
			var queryDict = BuildQueryDict(new VatQuery(input.NetEur, null, null, input.VatRatePercent));
			var response = await Query(queryDict, client);
			ValidateResponse(input, response);
			queryDict = BuildQueryDict(new VatQuery(null, input.GrossEur, null, input.VatRatePercent));
			response = await Query(queryDict, client);
			ValidateResponse(input, response);
			queryDict = BuildQueryDict(new VatQuery(null, null, input.VatAmountEur, input.VatRatePercent));
			response = await Query(queryDict, client);
			ValidateResponse(input, response);
		}
	}
	private static Dictionary<string, string?> BuildQueryDict(VatQuery query) {
		var queryDict = new Dictionary<string, string?>();
		if (query.NetEur.HasValue) queryDict.Add("netEur", query.NetEur?.ToString(CultureInfo.InvariantCulture));
		if (query.GrossEur.HasValue) queryDict.Add("grossEur", query.GrossEur?.ToString(CultureInfo.InvariantCulture));
		if (query.VatAmountEur.HasValue) queryDict.Add("vatAmountEur", query.VatAmountEur?.ToString(CultureInfo.InvariantCulture));
		queryDict.Add("vatRatePercent", query.VatRatePercent.ToString(CultureInfo.InvariantCulture));
		return queryDict;
	}
	private static async Task<VatResponse?> Query(Dictionary<string, string?> query, HttpClient client) {
		var urlWithQuery = QueryHelpers.AddQueryString("/vat", query);
		var response = await client.GetAsync(urlWithQuery);
		response.EnsureSuccessStatusCode();
		return await response.Content.ReadFromJsonAsync<VatResponse>();
	}
	private static void ValidateResponse(VatQuery expected, VatResponse? response) {
		Assert.NotNull(response);
		Assert.Equal(expected.NetEur, response.NetEur);
		Assert.Equal(expected.GrossEur, response.GrossEur);
		Assert.Equal(expected.VatAmountEur, response.VatAmountEur);
		Assert.Equal(expected.VatRatePercent, response.VatRatePercent);
	}
	[Fact]
	public async Task ThrowMissingOrInvalidAmountInput() {
		var client = factory.CreateClient();
		var query = BuildQueryDict(new VatQuery(null, null, null, 10m));
		await ValidateError(query, client, nameof(InvalidAmountException));
	}
	private static async Task ValidateError(Dictionary<string, string?> query, HttpClient client, string title) {
		var urlWithQuery = QueryHelpers.AddQueryString("/vat", query);
		var response = await client.GetAsync(urlWithQuery);
		var errorResponse = await response.Content.ReadFromJsonAsync<ProblemDetails>();
		Assert.NotNull(errorResponse);
		Assert.Equal(title, errorResponse.Title);
		Assert.Equal(400, errorResponse.Status);
		Assert.Equal("/vat", errorResponse.Instance);
		Assert.NotNull(errorResponse.Extensions["traceId"]);
	}
	[Fact]
	public async Task ThrowMoreThanOneInput() {
		var client = factory.CreateClient();
		foreach (var input in MoreThanOneInput) {
			var query = BuildQueryDict(input);
			await ValidateError(query, client, nameof(MoreThanOneInputException));
		}
	}
	[Fact]
	public async Task ThrowInvalidVatRateInput() {
		var client = factory.CreateClient();
		foreach (var input in InvalidVatRateInput) {
			var query = BuildQueryDict(input);
			await ValidateError(query, client, nameof(InvalidVatRateException));
		}
	}
	private IEnumerable<Dictionary<string, string?>> BuildSuccessQueryDict() {
		foreach (var input in SuccessInput) {
			yield return BuildQueryDict(input);
		}
	}
	[Fact]
	public async Task ThrowNonNumericInput() {
		var nonNumeric = BuildSuccessQueryDict().ToList();
		nonNumeric[0]["netEur"] = "a";
		nonNumeric[1]["grossEur"] = " ";
		nonNumeric[2]["vatAmountEur"] = "...";
		nonNumeric[3]["vatRatePercent"] = "12x";
		var client = factory.CreateClient();
		foreach (var query in nonNumeric) {
			await ValidateError(query, client, nameof(BadHttpRequestException));
		}
	}
}
