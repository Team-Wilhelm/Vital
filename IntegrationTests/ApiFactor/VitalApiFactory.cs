using Microsoft.AspNetCore.Mvc.Testing;
using Vital;

namespace IntegrationTests.ApiFactor;

public class VitalApiFactory : WebApplicationFactory<IApiAssemblyMarker>;
