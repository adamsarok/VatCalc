using static VatCalc.API.Features.VatCalc.GetVat;

namespace Vatcalc.Tests.UnitTests.VatCalc;
public class VatCalc {

	[Fact]
	public async Task GetVatAmounts() {
		var inputs = new List<VatQuery>() {
			new VatQuery(100m, 110m, 10m, 10m),
			new VatQuery(13m, 14.69m, 1.69m, 13m),
			new VatQuery(150000m, 180000m, 30000m, 20m),
			new VatQuery(-100m, -110m, -10m, 10m),
		};
		var handler = new VatHandler();
		foreach (var input in inputs) {
			var netInputResponse = await handler.Handle(new VatQuery(input.NetEur, null, null, input.VatRatePercent), new CancellationToken());
			ValidateResult(input, netInputResponse);
			var grossInputResponse = await handler.Handle(new VatQuery(null, input.GrossEur, null, input.VatRatePercent), new CancellationToken());
			ValidateResult(input, grossInputResponse);
			var vatAmountInputResponse = await handler.Handle(new VatQuery(null, null, input.VatAmountEur, input.VatRatePercent), new CancellationToken());
			ValidateResult(input, vatAmountInputResponse);
		}
	}
	private static void ValidateResult(VatQuery expected, VatResult response) {
		Assert.Equal(expected.NetEur, response.NetEur);
		Assert.Equal(expected.GrossEur, response.GrossEur);
		Assert.Equal(expected.VatAmountEur, response.VatAmountEur);
		Assert.Equal(expected.VatRatePercent, response.VatRatePercent);
	}
	[Fact]
	public async Task ThrowMissingOrInvalidAmountInput() {
		var handler = new VatHandler();
		await Assert.ThrowsAsync<InvalidAmountException>(async () => {
			await handler.Handle(new VatQuery(null, null, null, 20), new CancellationToken());
		});
	}
	[Fact]
	public async Task ThrowMoreThanOneInput() {
		var inputs = new List<VatQuery>() {
			new VatQuery(100m, 100m, null, 10m),
			new VatQuery(13m, null, 1.69m, 13m),
			new VatQuery(null, 180000m, 30000m, 20m)
		};
		var handler = new VatHandler();
		foreach (var input in inputs) {
			await Assert.ThrowsAsync<MoreThanOneInputException>(async () => {
				await handler.Handle(input, new CancellationToken());
			});
		}
	}
	[Fact]
	public async Task ThrowInvalidVatRateInput() {
		var inputs = new List<VatQuery>() {
			new VatQuery(100m, 10m, null, 10.1m),
			new VatQuery(13m, null, 1.69m, 10000m),
			new VatQuery(null, 180000m, 30000m, -5m),
			new VatQuery(null, 180000m, 30000m, 0m),
		};
		var handler = new VatHandler();
		foreach (var input in inputs) {
			await Assert.ThrowsAsync<InvalidVatRateException>(async () => {
				await handler.Handle(input, new CancellationToken());
			});
		}
	}
}
