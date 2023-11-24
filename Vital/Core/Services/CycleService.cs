using AutoMapper;
using Infrastructure.Repository.Interface;
using Models;
using Models.Days;
using Models.Dto.Cycle;
using Models.Pagination;
using Vital.Core.Context;
using Vital.Core.Services.Interfaces;
using Vital.Models.Exception;

namespace Vital.Core.Services;

public class CycleService : ICycleService
{
    private readonly ICycleRepository _cycleRepository;
    private readonly IMapper _mapper;
    private readonly CurrentContext _currentContext;
    private readonly ICalendarDayRepository _calendarDayRepository;

    public CycleService(ICycleRepository cycleRepository, ICalendarDayRepository calendarDayRepository, IMapper mapper,
        CurrentContext currentContext)
    {
        _cycleRepository = cycleRepository;
        _calendarDayRepository = calendarDayRepository;
        _mapper = mapper;
        _currentContext = currentContext;
    }

    public async Task<PaginatedList<Cycle>> Get(Paginator paginator)
    {
        return await _cycleRepository.Get(paginator);
    }

    public async Task<Cycle?> GetById(Guid id)
    {
        return await _cycleRepository.GetById(id);
    }

    public async Task<Cycle> Create()
    {
        var cycle = new Cycle()
        {
            StartDate = DateTimeOffset.Now,
            EndDate = null
        };
        cycle.Id = Guid.NewGuid();
        cycle.UserId = _currentContext.UserId!.Value;
        await _cycleRepository.Create(cycle);

        return cycle;
    }

    public async Task<Cycle> Update(Guid id, UpdateCycleDto dto)
    {
        var cycle = await _cycleRepository.GetById(id);
        if (cycle is null)
        {
            throw new NotFoundException();
        }

        _mapper.Map(dto, cycle);
        await _cycleRepository.Update(cycle);

        return cycle;
    }

    public async Task<List<DateTimeOffset>> GetPredictedPeriod(Guid userId)
    {
        var currentCycle = await GetCurrentCycle(userId);
        if (currentCycle is null)
        {
            throw new NotFoundException("No current cycle found.");
        }
        
        var predictedPeriodDays = new List<DateTimeOffset>();
        
        // Check if the user is on period and predicted when it should end based on the average period length.
        var periodDaysPassed = currentCycle.CycleDays.Count(day => day.IsPeriod); 
        if (periodDaysPassed is < 5 and >= 1) //TODO: Change hardcoded value
        {
            for (var i = 0; i < 5 - periodDaysPassed; i++)
            {
                predictedPeriodDays.Add(DateTimeOffset.Now.AddDays(i + 1));
            }
            return predictedPeriodDays;
        }
        
        
        // Otherwise the predicted period is calculated based on the average cycle length after the last period started.
        // The average cycle length is calculated based on the last 3 cycles.
        currentCycle.CycleDays = currentCycle.CycleDays.OrderBy(x => x.Date).ToList();
        for (var i = -2; i < 3; i++)
        {
            predictedPeriodDays.Add(currentCycle.CycleDays.Last().Date.AddDays(28 + i));
        }

        // TODO: change
        return predictedPeriodDays;
    }

    public async Task<Cycle?> GetCurrentCycle(Guid userId)
    {
        var cycle = await _cycleRepository.GetCurrentCycle(userId);
        if (cycle is null)
        {
            throw new NotFoundException("No current cycle found.");
        }

        var cycleDays =
            (List<CycleDay>)await _calendarDayRepository.GetCycleDaysForSpecifiedPeriodAsync(userId, cycle.StartDate,
                DateTimeOffset.UtcNow);
        cycle.CycleDays = cycleDays;

        return cycle;
    }
}
