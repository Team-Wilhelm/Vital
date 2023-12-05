using Models;
using Models.Dto.Cycle;
using Models.Pagination;

namespace Infrastructure.Repository.Interface;

/// <summary>
/// Provides an interface defining methods for accessing Cycle data.
/// </summary>
public interface ICycleRepository
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

    /// <summary>
    /// Creates a new Cycle object in the database.
    /// </summary>
    /// <param name="cycle">The Cycle object that should be added to the database.</param>
    /// <returns>A task that represents the asynchronous operation. 
    /// The task result contains the Cycle object that was added.</returns>
    Task<Cycle> Create(Cycle cycle);

    /// <summary>
    /// Updates an existing Cycle object in the database with new values.
    /// </summary>
    /// <param name="cycle">The Cycle object containing the updated values.</param>
    /// <returns>A task that represents the asynchronous operation. 
    /// The task result contains the updated Cycle object.</returns>
    Task<Cycle> Update(Cycle cycle);

    /// <summary>
    /// Retrieves the current cycle for the specified user.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<Cycle?> GetCurrentCycle(Guid userId);

    /// <summary>
    /// Retrrieves the period and cycle lengths for the last x number of cycles for the specified user.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="numberOfCycles"></param>
    /// <returns></returns>
    Task<List<PeriodAndCycleLengthDto>> GetPeriodAndCycleLengths(Guid userId, int numberOfCycles);

    /// <summary>
    /// Retrieves a list of x amount of most recent completed cycles for the specified user.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="numberOfCycles"></param>
    /// <returns></returns>
    Task<List<Cycle>> GetRecentCyclesWithDays(Guid userId, int numberOfCycles);
}
