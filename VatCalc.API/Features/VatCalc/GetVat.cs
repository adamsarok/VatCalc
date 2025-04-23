
using MediatR;

namespace VatCalc.API.Features.VatCalc;
public static class GetVat {
	public record VatQuery(decimal? NetEur, decimal? GrossEur, decimal? VatAmountEur, decimal VatRatePercentage)
		: IRequest<VatResponse>;
	public record VatResponse {
		static readonly decimal[] validVatRates = { 10, 13, 20 };
		public decimal? NetEur { get; }
		public decimal? GrossEur { get; } 
		public decimal? VatAmountEur { get; } 
		public decimal VatRatePercentage { get; }
		public VatResponse(decimal? netEur, decimal? grossEur, decimal? vatAmountEur, decimal vatRatePercentage) {
			if (!validVatRates.Contains(vatRatePercentage)) throw new InvalidVatRateException();
			if (netEur == 0 || grossEur == 0 || vatAmountEur == 0) throw new InvalidAmountException();
			var validInputCnt = new[] { netEur, grossEur, vatAmountEur }.Count(x => x != null);
			if (validInputCnt > 1) throw new MoreThanOneInputException();
			if (validInputCnt < 1) throw new InvalidAmountException();
			VatRatePercentage = vatRatePercentage;
			if (netEur.HasValue) {
				NetEur = netEur.Value;
				VatAmountEur = NetEur * VatRatePercentage / 100;
				GrossEur = NetEur + VatAmountEur;
			} else if (grossEur.HasValue) {
				GrossEur = grossEur.Value;
				NetEur = GrossEur / (1 + VatRatePercentage / 100);
				VatAmountEur = GrossEur - NetEur;
			} else if (vatAmountEur.HasValue) {
				VatAmountEur = vatAmountEur.Value;
				NetEur = VatAmountEur / (VatRatePercentage / 100);
				GrossEur = NetEur + VatAmountEur;
			} else throw new InvalidAmountException();
		}
	}

	public class VatHandler : IRequestHandler<VatQuery, VatResponse> {
		public async Task<VatResponse> Handle(VatQuery request, CancellationToken cancellationToken) {
			return new VatResponse(request.NetEur, request.GrossEur, request.VatAmountEur, request.VatRatePercentage);
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

