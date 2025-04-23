
using MediatR;

namespace VatCalc.API.Features.VatCalc;
public static class GetVat {
	public record VatQuery(decimal? NetEur, decimal? GrossEur, decimal? VatAmountEur, decimal VatRate)
		: IRequest<VatResponse>;
	public record VatResponse {
		static readonly decimal[] validVatRates = { 10, 13, 20 };
		public decimal? NetEur { get; }
		public decimal? GrossEur { get; } 
		public decimal? VatAmountEur { get; } 
		public decimal VatRate { get; }
		public VatResponse(decimal? netEur, decimal? grossEur, decimal? vatAmountEur, decimal vatRate) {
			if (!validVatRates.Contains(vatRate)) throw new InvalidVatRateException();
			if (netEur == 0 || grossEur == 0 || vatAmountEur == 0) throw new InvalidAmountException();
			var validInputCnt = new[] { netEur, grossEur, vatAmountEur }.Count(x => x != null);
			if (validInputCnt > 1) throw new MoreThanOneInputException();
			if (validInputCnt < 1) throw new InvalidAmountException();
			NetEur = netEur;
			GrossEur = grossEur;
			VatAmountEur = vatAmountEur;
			VatRate = vatRate;
			//TODO
			throw new NotImplementedException();
		}
	}

	public class VatHandler : IRequestHandler<VatQuery, VatResponse> {
		public async Task<VatResponse> Handle(VatQuery request, CancellationToken cancellationToken) {
			return new VatResponse(request.NetEur, request.GrossEur, request.VatAmountEur, request.VatRate);
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

