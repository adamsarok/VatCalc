
using static VatCalc.API.Features.VatCalc.GetVat;

namespace Vatcalc.Tests.Features.VatCalc;
public class VatCalcTestBase {
	protected List<VatQuery> SuccessInput = new List<VatQuery>() {
		new VatQuery(100m, 110m, 10m, 10m),
		new VatQuery(13m, 14.69m, 1.69m, 13m),
		new VatQuery(150000m, 180000m, 30000m, 20m),
		new VatQuery(-100m, -110m, -10m, 10m),
	};
	protected List<VatQuery> MoreThanOneInput = new List<VatQuery>() {
		new VatQuery(100m, 100m, null, 10m),
		new VatQuery(13m, null, 1.69m, 13m),
		new VatQuery(null, 180000m, 30000m, 20m)
	};
	protected List<VatQuery> InvalidVatRateInput = new List<VatQuery>() {
			new VatQuery(100m, 10m, null, 10.1m),
			new VatQuery(13m, null, 1.69m, 10000m),
			new VatQuery(null, 180000m, 30000m, -5m),
			new VatQuery(null, 180000m, 30000m, 0m),
		};
}

