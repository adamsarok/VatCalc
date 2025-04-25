using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.WebUtilities;
using System.Globalization;
using System.Net.Http.Json;
using static VatCalc.API.Features.VatCalc.GetVat;

namespace Vatcalc.Tests.SystemTests.VatCalc;
public class VatCalc(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>> {

	[Fact]
	public async Task GetVatAmounts() {
		var inputs = new List<VatQuery>() {
			new VatQuery(100m, 110m, 10m, 10m),
			new VatQuery(13m, 14.69m, 1.69m, 13m),
			new VatQuery(150000m, 180000m, 30000m, 20m),
			new VatQuery(-100m, -110m, -10m, 10m),
		};
		var client = factory.CreateClient();
		foreach (var input in inputs) {
			var query = new Dictionary<string, string?> {
				["netEur"] = input.NetEur?.ToString(CultureInfo.InvariantCulture),
				["vatRatePercent"] = input.VatRatePercent.ToString(CultureInfo.InvariantCulture),
			};
			var response = await Query(query, client);
			ValidateResponse(input, response);
			query = new Dictionary<string, string?> {
				["grossEur"] = input.GrossEur?.ToString(CultureInfo.InvariantCulture),
				["vatRatePercent"] = input.VatRatePercent.ToString(CultureInfo.InvariantCulture),
			};
			response = await Query(query, client);
			ValidateResponse(input, response);
			query = new Dictionary<string, string?> {
				["vatAmountEur"] = input.VatAmountEur?.ToString(CultureInfo.InvariantCulture),
				["vatRatePercent"] = input.VatRatePercent.ToString(CultureInfo.InvariantCulture),
			};
			response = await Query(query, client);
			ValidateResponse(input, response);
		}
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
}
