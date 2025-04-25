namespace VatCalc.API.Features.VatCalc;
public static class GetVat {
	public record VatResponse(decimal? NetEur, decimal? GrossEur, decimal? VatAmountEur, decimal VatRatePercent);
	public record VatQuery(decimal? NetEur, decimal? GrossEur, decimal? VatAmountEur, decimal VatRatePercent)
		: IRequest<VatResult>;
	public record VatResult {
		static readonly decimal[] validVatRates = { 10, 13, 20 };
		public decimal? NetEur { get; }
		public decimal? GrossEur { get; }
		public decimal? VatAmountEur { get; }
		public decimal VatRatePercent { get; }
		public VatResult(decimal? netEur, decimal? grossEur, decimal? vatAmountEur, decimal vatRatePercent) {
			if (!validVatRates.Contains(vatRatePercent)) throw new InvalidVatRateException();
			if (netEur == 0 || grossEur == 0 || vatAmountEur == 0) throw new InvalidAmountException();
			var validInputCnt = new[] { netEur, grossEur, vatAmountEur }.Count(x => x != null);
			if (validInputCnt > 1) throw new MoreThanOneInputException();
			if (validInputCnt < 1) throw new InvalidAmountException();
			VatRatePercent = vatRatePercent;
			if (netEur.HasValue) {
				NetEur = netEur.Value;
				VatAmountEur = NetEur * VatRatePercent / 100;
				GrossEur = NetEur + VatAmountEur;
			} else if (grossEur.HasValue) {
				GrossEur = grossEur.Value;
				NetEur = GrossEur / (1 + VatRatePercent / 100);
				VatAmountEur = GrossEur - NetEur;
			} else if (vatAmountEur.HasValue) {
				VatAmountEur = vatAmountEur.Value;
				NetEur = VatAmountEur / (VatRatePercent / 100);
				GrossEur = NetEur + VatAmountEur;
			} else throw new InvalidAmountException();
		}
	}

	public class VatHandler : IRequestHandler<VatQuery, VatResult> {
		public async Task<VatResult> Handle(VatQuery request, CancellationToken cancellationToken) {
			return new VatResult(request.NetEur, request.GrossEur, request.VatAmountEur, request.VatRatePercent);
		}
	}
	public class VatModule : ICarterModule {
		public void AddRoutes(IEndpointRouteBuilder app) {
			app.MapGet("/vat", async ([FromQuery] decimal? netEur, decimal? grossEur, decimal? vatAmountEur, decimal vatRatePercent, ISender sender) => {
				var result = await sender.Send(new VatQuery(netEur, grossEur, vatAmountEur, vatRatePercent));
				var response = result.Adapt<VatResponse>();
				return Results.Ok(response);
			})
		.WithName("GetVatAmounts")
		.Produces<VatResponse>(StatusCodes.Status200OK)
		.ProducesProblem(StatusCodes.Status400BadRequest)
		.WithSummary("Get Vat Amounts")
		.WithDescription("Get Net/Gross/VAT amounts in EUR");
		}
	}
	public class InvalidAmountException : Exception {
		public InvalidAmountException() : base($"Missing or invalid amount input") { }
	}
	public class MoreThanOneInputException : Exception {
		public MoreThanOneInputException() : base("More than one input amount provided. Please set only one of gross, net, VAT amounts.") { }
	}
	public class InvalidVatRateException : Exception {
		public InvalidVatRateException() : base("Missing or invalid VAT rate input") { }
	}
}

