using AutoMapper;
using Infrastructure.Repository.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Models;
using Models.Days;
using Models.Dto.Cycle;
using Models.Dto.InitialLogin;
using Models.Dto.Metrics;
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
    private readonly IMetricService _metricService;

    public CycleService(ICycleRepository cycleRepository, ICalendarDayRepository calendarDayRepository, IMapper mapper,
        CurrentContext currentContext, UserManager<ApplicationUser> userManager, IMetricService metricService)
    {
        _cycleRepository = cycleRepository;
        _calendarDayRepository = calendarDayRepository;
        _mapper = mapper;
        _currentContext = currentContext;
        _userManager = userManager;
        _metricService = metricService;
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
        var user = await _userManager.FindByIdAsync(_currentContext.UserId!.Value.ToString());
        if (user == null)
        {
            throw new NotFoundException("No user found.");
        }
        
        var periodAndCycleLength = await CalculatePeriodAndCycleLength();
        user.CycleLength = periodAndCycleLength.CycleLength;
        user.PeriodLength = periodAndCycleLength.PeriodLength;
        await _userManager.UpdateAsync(user);
        
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
    
    public async Task<Cycle> Create(Cycle cycle)
    {
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
        
        var today = DateTime.UtcNow.Date;
        var predictedPeriodDays = new List<DateTimeOffset>();
        var user = _userManager.Users.First(u => u.Id == userId);
        var cycleLength = user.CycleLength;
        var periodLength = user.PeriodLength;
        var cycleStartDay = currentCycle.StartDate.Date;
        
        //get latest period day for current cycle and add predicted days based on period length
        var latestPeriodDay = currentCycle.CycleDays.Where(d => d.IsPeriod).OrderBy(d => d.Date).Last().Date;

        //Only add predicted period days if latest period day is less than three cycles ago
        if ((today - latestPeriodDay).TotalDays >= 3 * cycleLength) return predictedPeriodDays;
        var periodElapsed = (latestPeriodDay - cycleStartDay).Days +1;
        var difference = periodLength - periodElapsed;
        
        //Add predicted days after latest period day until cycle length is reached
        for (var i = 0; i < difference; i++)
        {
            var dayToAdd = latestPeriodDay.AddDays(i + 1);
            if (dayToAdd.Date > today)
            {
                predictedPeriodDays.Add(dayToAdd);
            }
        }
        
        //get predicted period days for the next three cycles
        for (var i = 0; i < 3; i++)
        {
            cycleStartDay = cycleStartDay.Date.AddDays(cycleLength!.Value);
            for (var j = 0; j < periodLength; j++)
            {
                predictedPeriodDays.Add(cycleStartDay.Date.AddDays(j));
            }
        }
        return predictedPeriodDays;
    }

    public async Task<List<CycleAnalyticsDto>> GetAnalytics(Guid userId, int numberOfCycles)
    {
        var cycleList = await _cycleRepository.GetRecentCyclesWithDays(userId, numberOfCycles);
        var cycleAnalytics = cycleList.Select(cycle => new CycleAnalyticsDto
        {
            StartDate = cycle.StartDate,
            EndDate = (DateTimeOffset)cycle.EndDate!, 
            PeriodDays = cycle.CycleDays
                .Where(cd => cd.IsPeriod)
                .Select(cd => cd.Date)
                .ToList()
        }).ToList();

        return cycleAnalytics;
    }

    public async Task<PeriodCycleStatsDto> GetPeriodCycleStats(Guid userId)
    {
        var currentCycle = await GetCurrentCycle(userId);
        var currentCycleLength = (DateTimeOffset.UtcNow - currentCycle!.StartDate).Days;
        return new PeriodCycleStatsDto
        {
            CurrentCycleLength = currentCycleLength,
            AverageCycleLength = (int) (_userManager.Users.FirstOrDefault(u => u.Id == userId)?.CycleLength ?? 0),
            AveragePeriodLength = (int) (_userManager.Users.FirstOrDefault(u => u.Id == userId)?.PeriodLength ?? 0)
        };
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

    public async Task SetInitialData(Guid userId, InitialLoginPutDto initialLoginPutDto)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user!.CurrentCycleId != null)
        {
            throw new BadRequestException("User has already set their initial data.");
        }
        
        // Create a new cycle for the user based on their last period date (it won't have an end date, as it's ongoing)
        var cycle = new Cycle()
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            StartDate = initialLoginPutDto.LastPeriodStart
        };
        await Create(cycle);
        
        // Update user's average cycle and period lengths
        user!.PeriodLength = initialLoginPutDto.PeriodLength;
        user!.CycleLength = initialLoginPutDto.CycleLength;
        user.CurrentCycleId = cycle.Id;
        await _userManager.UpdateAsync(user);
        
        // Log the period days for the cycle
        var periodDay = initialLoginPutDto.LastPeriodStart;
        var upperBound = initialLoginPutDto.LastPeriodEnd ??
                         initialLoginPutDto.LastPeriodStart.AddDays(initialLoginPutDto.PeriodLength);
        upperBound = upperBound > DateTimeOffset.UtcNow 
            ? DateTimeOffset.UtcNow 
            : upperBound;
        
        var metrics = new List<MetricRegisterMetricDto>();
        var metricId = (await _metricService.GetAllMetrics())
            .First(m => m.Name == "Flow")
            .Id;
        
        while (periodDay <= upperBound)
        {
           metrics.Add(new MetricRegisterMetricDto()
           {
               MetricsId = metricId,
               CreatedAt = periodDay,
           });
           periodDay = periodDay.AddDays(1);
        }
        
        await _metricService.SaveMetrics(userId, metrics);
    }
}
