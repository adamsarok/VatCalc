
namespace VatCalc.API.Features.VatCalc;
public static class GetVat {
	public record VatQuery(decimal? NetEur, decimal? GrossEur, decimal? VatAmountEur, decimal VatRate)
		: IRequest<VatResponse>;
	public record VatResponse(decimal? NetEur, decimal? GrossEur, decimal? VatAmountEur, decimal VatRate);
	public class VatHandler : IRequestHandler<VatQuery, VatResponse> {
		public Task<VatResponse> Handle(VatQuery request, CancellationToken cancellationToken) {
			throw new NotImplementedException();
		}
	}
	public class InvalidAmountException : Exception {
		public InvalidAmountException(string fieldName) : base($"Missing or invalid amount input") { }
	}
	public class MoreThanOneInputException : Exception {
		public MoreThanOneInputException() : base("More than one input amount provided. Please set only one of gross, net, VAT amounts.") { }
	}
	public class InvalidVatRateException : Exception {
		public InvalidVatRateException() : base("Missing or invalid VAT rate input") { }
	}
}

