using AutoMapper;
using Infrastructure.Repository.Interface;
using Microsoft.AspNetCore.Identity;
using Models;
using Models.Days;
using Models.Dto.Cycle;
using Models.Identity;
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
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICalendarDayRepository _calendarDayRepository;

    public CycleService(ICycleRepository cycleRepository, ICalendarDayRepository calendarDayRepository, IMapper mapper,
        CurrentContext currentContext, UserManager<ApplicationUser> userManager)
    {
        _cycleRepository = cycleRepository;
        _calendarDayRepository = calendarDayRepository;
        _mapper = mapper;
        _currentContext = currentContext;
        _userManager = userManager;
    }

    /// <summary>
    /// Get all cycles paginated for the current user.
    /// </summary>
    /// <param name="paginator"></param>
    /// <returns></returns>
    public async Task<PaginatedList<Cycle>> Get(Paginator paginator)
    {
        return await _cycleRepository.Get(paginator);
    }

    /// <summary>
    /// Get a cycle by its id.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<Cycle?> GetById(Guid id)
    {
        return await _cycleRepository.GetById(id);
    }

    /// <summary>
    /// Create a new cycle for the current user and re-calculates average period and cycle length.
    /// </summary>
    /// <returns></returns>
    public async Task<Cycle> Create()
    {
        if (_currentContext.UserId == null)
        {
            throw new NotFoundException("No user found.");
        }
        
        var periodAndCycleLength = await CalculatePeriodAndCycleLength();
        _userManager.Users.First(u => u.Id == _currentContext.UserId.Value).CycleLength = periodAndCycleLength.CycleLength;
        _userManager.Users.First(u => u.Id == _currentContext.UserId.Value).PeriodLength = periodAndCycleLength.PeriodLength;
        
        var cycle = new Cycle
        {
            StartDate = DateTimeOffset.Now,
            EndDate = null,
            Id = Guid.NewGuid(),
            UserId = _currentContext.UserId!.Value
        };
        await _cycleRepository.Create(cycle);

        return cycle;
    }

    /// <summary>
    /// Update a cycle by its id.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="dto"></param>
    /// <returns></returns>
    /// <exception cref="NotFoundException"></exception>
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
    
    /// <summary>
    /// Calculates average period and cycle length for the current user based on up to last three cycles.
    /// </summary>
    public async Task<PeriodAndCycleLengthDto> CalculatePeriodAndCycleLength()
    {
        var periodAndCycleLengths = await _cycleRepository.GetPeriodAndCycleLengths(_currentContext.UserId!.Value, 3);
        var cycleLength = periodAndCycleLengths.Select(p => p.CycleLength).Average();
        var periodLength = periodAndCycleLengths.Select(p => p.PeriodLength).Average();
        var periodAndCycleLength = new PeriodAndCycleLengthDto
        {
            CycleLength = (float)Math.Round(cycleLength),
            PeriodLength = (float)Math.Round(periodLength)
        };
        return periodAndCycleLength;
    }

    /// <summary>
    /// Calculates remaining days in current cycle and predicted period days for the next three cycles.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    /// <exception cref="NotFoundException"></exception>
    public async Task<List<DateTimeOffset>> GetPredictedPeriod(Guid userId)
    {
        var currentCycle = await GetCurrentCycle(userId);
        if (currentCycle is null)
        {
            throw new NotFoundException("No current cycle found.");
        }
        
        var predictedPeriodDays = new List<DateTimeOffset>();
        var cycleLength = _userManager.Users.First(u => u.Id == userId).CycleLength;
        var periodLength = _userManager.Users.First(u => u.Id == userId).PeriodLength;
        var cycleStartDay = currentCycle.StartDate;
        
        //get latest period day for current cycle and add predicted days based on period length
        var latestPeriodDay = currentCycle.CycleDays.Where(d => d.IsPeriod).OrderBy(d => d.Date).Last();
        //TODO if latestPeriodDay is more than 3 cyclelengths ago don't add predicted days
        var difference = (int)(latestPeriodDay.Date - cycleStartDay).TotalDays;
        
        for (var i = 0; i < difference; i++)
        {
            predictedPeriodDays.Add(latestPeriodDay.Date.AddDays(i + 1));
        }
        
        //get predicted period days for the next three cycles
        for (var i = 1; i < 4; i++)
        {
            var futureCycleStartDay = cycleStartDay.Date.AddDays(cycleLength*i);
            for (var j = 0; j < periodLength; j++)
            {
                predictedPeriodDays.Add(futureCycleStartDay.Date.AddDays(cycleLength + j));
            }
        }
        
        return predictedPeriodDays;
    }

    /// <summary>
    /// Get current cycle for the current user.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    /// <exception cref="NotFoundException"></exception>
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
