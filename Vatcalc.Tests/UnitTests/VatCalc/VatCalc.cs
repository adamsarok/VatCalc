using static VatCalc.API.Features.VatCalc.GetVat;

namespace Vatcalc.Tests.UnitTests.VatCalc;
public class VatCalc {

	[Theory]
	[InlineData(100, 10, 110, 10)]
	[InlineData(13, 1.69, 14.69, 13)]
	[InlineData(150000, 30000, 180000, 20)]
	public async Task Calc(decimal netValueEur, decimal grossValueEur, decimal vatAmountEur, decimal vatRate) {
		var handler = new VatHandler();
		var netInputResponse = await handler.Handle(new VatQuery(netValueEur, null, null, vatRate), new CancellationToken());
		ValidateResponse(netValueEur, grossValueEur, vatAmountEur, netInputResponse);
		var grossInputResponse = await handler.Handle(new VatQuery(null, grossValueEur, null, vatRate), new CancellationToken());
		ValidateResponse(netValueEur, grossValueEur, vatAmountEur, grossInputResponse);
		var vatAmountInputResponse = await handler.Handle(new VatQuery(null, null, vatAmountEur, vatRate), new CancellationToken());
		ValidateResponse(netValueEur, grossValueEur, vatAmountEur, vatAmountInputResponse);
	}
	private static void ValidateResponse(decimal netValueEur, decimal grossValueEur, decimal vatAmountEur, VatResponse response) {
		Assert.Equal(netValueEur, response.NetEur);
		Assert.Equal(grossValueEur, response.GrossEur);
		Assert.Equal(vatAmountEur, response.VatAmountEur);
	}
	[Fact]
	public async Task MissingOrInvalidAmountInput() {
		var handler = new VatHandler();
		await Assert.ThrowsAsync<InvalidAmountException>(async () => {
			await handler.Handle(new VatQuery(null, null, null, 20), new CancellationToken());
		});
	}
	[Fact]
	public async Task MoreThanOneInput() {
		var inputs = new List<VatQuery>() {
			new VatQuery(100m, 10m, null, 10m),
			new VatQuery(13m, null, 14.69m, 13m),
			new VatQuery(null, 30000m, 180000m, 20m)
		};
		var handler = new VatHandler();
		foreach (var input in inputs) {
			await Assert.ThrowsAsync<MoreThanOneInputException>(async () => {
				await handler.Handle(input, new CancellationToken());
			});
		}
	}
	[Fact]
	public async Task InvalidVatRateInput() {
		var inputs = new List<VatQuery>() {
			new VatQuery(100m, 10m, null, 10.1m),
			new VatQuery(13m, null, 14.69m, 10000m),
			new VatQuery(null, 30000m, 180000m, -5m),
			new VatQuery(null, 30000m, 180000m, 0m),
		};
		var handler = new VatHandler();
		foreach (var input in inputs) {
			await Assert.ThrowsAsync<InvalidVatRateException>(async () => {
				await handler.Handle(input, new CancellationToken());
			});
		}
	}
}
