using static VatCalc.API.Features.VatCalc.GetVat;

namespace VatCalc.Tests.Features.VatCalc;
public class VatCalcUnitTests : VatCalcTestBase {

	[Fact]
	public async Task GetVatAmounts() {
		var handler = new VatHandler();
		foreach (var input in SuccessInput) {
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
		var handler = new VatHandler();
		foreach (var input in MoreThanOneInput) {
			await Assert.ThrowsAsync<MoreThanOneInputException>(async () => {
				await handler.Handle(input, new CancellationToken());
			});
		}
	}
	[Fact]
	public async Task ThrowInvalidVatRateInput() {
		var handler = new VatHandler();
		foreach (var input in InvalidVatRateInput) {
			await Assert.ThrowsAsync<InvalidVatRateException>(async () => {
				await handler.Handle(input, new CancellationToken());
			});
		}
	}
}
