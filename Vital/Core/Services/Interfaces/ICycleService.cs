using Models;
using Models.Dto.Cycle;
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

    /// <summary>
    /// Creates a new Cycle object with the specified details.
    /// </summary>
    /// <param name="dto">A CreateCycleDto object containing the details for the new Cycle object.</param>
    /// <returns>A task that represents the asynchronous operation. 
    /// The task result contains the Cycle object that was added.</returns>
    Task<Cycle> Create(CreateCycleDto dto);

    /// <summary>
    /// Updates an existing Cycle object with new values.
    /// </summary>
    /// <param name="id">The unique identifier of the Cycle object to be updated.</param>
    /// <param name="dto">An UpdateCycleDto object containing the updated values.</param>
    /// <returns>A task that represents the asynchronous operation. 
    /// The task result contains the updated Cycle object.</returns>
    Task<Cycle> Update(Guid id, UpdateCycleDto dto);

    /// <summary>
    /// Gets a list of predicted period days for the specified cycle.
    /// </summary>
    /// <param name="cycleId">The unique ID of the specified cycle.</param>
    /// <returns>A task that represents the asynchronous operation. 
    /// The task result contains a list of PredictedPeriodDayDtos.</returns>
    Task<List<DateTimeOffset>> GetPredictedPeriod(Guid userId);
    
    /// <summary>
    /// Retrieves the current cycle for the specified user.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<Cycle?> GetCurrentCycle(Guid userId);
}
