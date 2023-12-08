using Microsoft.AspNetCore.Mvc;

namespace Vital.Controllers;

[ApiController]
[Route("[controller]")]
public abstract class BaseController : ControllerBase;
