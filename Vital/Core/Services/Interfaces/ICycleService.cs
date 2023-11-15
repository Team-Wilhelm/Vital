using Models;
using Models.Pagination;

namespace Vital.Core.Services.Interfaces;

/// <summary>
/// Provides an interface defining methods for accessing Cycle data.
/// </summary>
public interface ICycleService
{
    /// <summary>
    /// Gets a paginated list of Cycle objects.
    /// </summary>
    /// <param name="paginator">An object of type Paginator which contains parameters for pagination.</param>
    /// <returns>A task that represents the asynchronous operation. 
    /// The task result contains a PaginatedList of Cycle objects.</returns>
    Task<PaginatedList<Cycle>> Get(Paginator paginator);

    /// <summary>
    /// Gets a Cycle object by its ID.
    /// </summary>
    /// <param name="id">The unique Guid identifier of the Cycle object.</param>
    /// <returns>A task that represents the asynchronous operation. 
    /// The task result contains a Cycle object, or null if no objects were found with the provided ID.</returns>
    Task<Cycle?> GetById(Guid id);
}
