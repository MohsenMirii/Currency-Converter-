#region

using Microsoft.AspNetCore.Mvc;

#endregion

namespace API.Constraints;

/// <summary>
///     NOTE: if you change this lines, you may need to update FileController
/// </summary>
[Consumes("application/json")]
[Produces("application/json")]
[ProducesResponseType(StatusCodes.Status200OK)]
[Route($"{GlobalConstants.ApiPrefix}/[controller]")]
[ApiController]
public class BaseController : ControllerBase {
}