using static VatCalc.API.Features.VatCalc.GetVat;

namespace Vatcalc.Tests {
	public class UnitTests {

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
		[Theory]
		[InlineData(100, 10, null, 10)]
		[InlineData(13, null, 14.69, 13)]
		[InlineData(null, 30000, 180000, 20)]
		public async Task MoreThanOneInput(decimal? netValueEur, decimal? grossValueEur, decimal? vatAmountEur, decimal vatRate) {
			var handler = new VatHandler();
			await Assert.ThrowsAsync<MoreThanOneInputException>(async () => {
				await handler.Handle(new VatQuery(netValueEur, grossValueEur, vatAmountEur, vatRate), new CancellationToken());
			});
		}
		[Theory]
		[InlineData(100, null, null, 10.1)]
		[InlineData(null, 1.69, null, 10000)]
		[InlineData(null, null, 180000, -5)]
		public async Task MissingVatRateInput(decimal? netValueEur, decimal? grossValueEur, decimal? vatAmountEur, decimal vatRate) {
			var handler = new VatHandler();
			await Assert.ThrowsAsync<InvalidVatRateException>(async () => {
				await handler.Handle(new VatQuery(netValueEur, grossValueEur, vatAmountEur, vatRate), new CancellationToken());
			});
		}
	}
}
