using System.Globalization;
using AutoMapper;
using Infrastructure.Adapters;
using Infrastructure.Repository.Interface;
using Models;
using Models.Days;
using Models.Dto.Cycle;
using Models.Dto.Metrics;
using Models.Util;
using Org.BouncyCastle.Asn1.X509;
using Vital.Core.Services.Interfaces;
using Vital.Models.Exception;

namespace Vital.Core.Services;

public class MetricService : IMetricService
{
    private readonly IMetricRepository _metricRepository;
    private readonly ICalendarDayRepository _calendarDayRepository;
    private readonly ICycleRepository _cycleRepository;
    private readonly IMapper _mapper;

    public MetricService(IMetricRepository metricRepository, ICalendarDayRepository calendarDayRepository,
        IMapper mapper, ICycleRepository cycleRepository)
    {
        _metricRepository = metricRepository;
        _calendarDayRepository = calendarDayRepository;
        _mapper = mapper;
        _cycleRepository = cycleRepository;
    }

    public async Task<List<Metrics>> GetAllMetrics()
    {
        return await _metricRepository.GetAllMetrics();
    }

    public async Task<IEnumerable<CalendarDay>> GetMetricsForCalendarDays(Guid userId, DateTimeOffset fromDate,
        DateTimeOffset toDate)
    {
        var list = await _metricRepository.GetMetricsForCalendarDays(userId, fromDate, toDate);

        var calendarDays = new List<CalendarDay>();
        foreach (var calendarDayAdapter in list)
        {
            // Check if the day already exists in the list
            var calendarDay = calendarDays.FirstOrDefault(c => c.Id == calendarDayAdapter.CalendarDayId);
            if (calendarDay is null)
            {
                calendarDay = BuildCalendarDayFromAdapter(calendarDayAdapter);
                if (calendarDay != null) calendarDays.Add(calendarDay);
            }

            // Create the metric and its value and append to the day's list of selected metrics
            var calendarDayMetric = new CalendarDayMetric()
            {
                Id = calendarDayAdapter.CalendarDayMetricId,
                CalendarDayId = calendarDayAdapter.CalendarDayId,
                MetricValueId = calendarDayAdapter.MetricValueId,
                MetricsId = calendarDayAdapter.MetricsId,
                CreatedAt = calendarDayAdapter.CreatedAt,
            };

            var metric = new Metrics()
            {
                Id = calendarDayAdapter.MetricsId,
                Name = calendarDayAdapter.MetricName,
                Values = new List<MetricValue?>()
            };

            if (calendarDayAdapter.MetricValueId is not null)
            {
                var metricValue = new MetricValue()
                {
                    Id = calendarDayAdapter.MetricValueId!.Value,
                    Name = calendarDayAdapter.MetricValueName,
                    MetricsId = calendarDayAdapter.MetricsId,
                };
                calendarDayMetric.MetricValue = metricValue;
                metric.Values.Add(metricValue);
            }

            calendarDayMetric.Metrics = metric;
            calendarDay?.SelectedMetrics.Add(calendarDayMetric);
        }

        return calendarDays;
    }

    public async Task<ICollection<CalendarDayMetric>> Get(Guid userId, DateTimeOffset date)
    {
        var list = await _metricRepository.Get(userId, date);
        return list;
    }

    public async Task<IEnumerable<DateTimeOffset>> GetPeriodDays(Guid userId, DateTimeOffset fromDate,
        DateTimeOffset toDate)
    {
        var list = await _metricRepository.GetPeriodDays(userId, fromDate, toDate);
        return list;
    }

    /// <summary>
    /// Deletes any existing metrics for the day and uploads the new metrics. Does it???
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="metrics"></param>
    /// <exception cref="BadRequestException"></exception>
    public async Task SaveMetrics(Guid userId, List<MetricRegisterMetricDto> metrics)
    {
        var dayList = metrics.Select(m => m.CreatedAt).Distinct().OrderBy(d => d).ToList();
        if (dayList.Any(date => date.Date > DateTimeOffset.UtcNow.Date))
        {
            throw new BadRequestException("Cannot log metrics for a future date.");
        }

        foreach (var date in dayList)
        {
            var cycle = await _cycleRepository.GetCycleByDate(userId, date);
            var currentCycle = await _cycleRepository.GetCurrentCycle(userId);
            var metricNames =
                await _metricRepository.GetMetricNamesByIds(metrics.Select(m => m.MetricsId)
                    .ToList());

            // Saving data for the current cycle
            if (cycle is not null && currentCycle is not null && cycle.Id == currentCycle.Id)
            {
                var calendarDay = await GetOrCreateCalendarDay(userId, date, cycle.Id);
                if (metricNames.Values.Any(m => m.Contains("Flow")))
                {
                    await SetIsPeriodOnCalendarDay(calendarDay);
                }

                await _metricRepository.SaveMetrics(calendarDay.Id, metrics.Where(m => m.CreatedAt == date).ToList());
            } else if (cycle is not null)
            {
                // If the cycle is not null, but it's not the current cycle, then we need to check if the metric being savied is 'Flow' and if so, 
                // check if the metric's date is more than 2 days from the retrieved cycle's last 'Flow' metric date, which should create a new cycle
                if (metricNames.Values.Any(m => m.Contains("Flow")))
                {
                    var lastFlowMetricDate = await _metricRepository
                        .GetPeriodDays(userId, cycle.StartDate, cycle.EndDate ?? DateTimeOffset.Now)
                        .ContinueWith(t => t.Result.OrderByDescending(d => d).FirstOrDefault());

                    var followingCycle = await _cycleRepository.GetFollowingCycle(userId, date);
                    if (date.Date - lastFlowMetricDate.Date >
                        TimeSpan.FromDays(
                            3)) // This case is for handling logging flow, which is less than 2 days from the next logged flow
                    {
                        // If the date is within 2 days of the following cycle's start date, merge the cycle with the following cycle
                        if (followingCycle is not null && followingCycle.StartDate - date <= TimeSpan.FromDays(2))
                        {
                            // Update the start of following cycle
                            await _cycleRepository.Update(new Cycle()
                            {
                                Id = followingCycle.Id,
                                UserId = userId,
                                StartDate = date,
                                EndDate = followingCycle.EndDate
                            });

                            // If the following cycle contains any metrics, which now belong to the new cycle, update their cycle id
                            await _calendarDayRepository.UpdateCycleIds(followingCycle.Id, followingCycle.Id, date,
                                followingCycle.EndDate ?? DateTimeOffset.UtcNow);

                            // Update the end of the retrieved cycle
                            await _cycleRepository.Update(new Cycle()
                            {
                                Id = cycle.Id,
                                UserId = userId,
                                StartDate = cycle.StartDate,
                                EndDate = date.AddDays(-1)
                            });
                            cycle = followingCycle;
                        } else if (followingCycle is not null)
                        {
                            // Otherwise, create a new cycle with the metric date as the start date and the following cycle's start date as the end date
                            var newCycle = await _cycleRepository.Create(new Cycle()
                            {
                                Id = Guid.NewGuid(),
                                UserId = userId,
                                StartDate = date,
                                EndDate = followingCycle.StartDate.AddDays(-1)
                            });

                            // and update the previous cycle's end date to be the day before the new cycle's start date
                            await _cycleRepository.Update(new Cycle()
                            {
                                Id = cycle.Id,
                                UserId = userId,
                                StartDate = cycle.StartDate,
                                EndDate = newCycle.StartDate.AddDays(-1)
                            });

                            // If the following cycle contains any metrics, which now belong to the new cycle, update their cycle id
                            await _calendarDayRepository.UpdateCycleIds(cycle.Id, newCycle.Id, cycle.StartDate,
                                cycle.EndDate!.Value);
                            cycle = newCycle;
                        }
                    }
                }

                var calendarDay = await GetOrCreateCalendarDay(userId, date, cycle.Id);
                if (metricNames.Values.Any(m => m.Contains("Flow")))
                {
                    await SetIsPeriodOnCalendarDay(calendarDay);
                }

                await _metricRepository.SaveMetrics(calendarDay.Id, metrics.Where(m => m.CreatedAt == date).ToList());
            } else
            {
                // This case means there is no historic cycle (which old enough to contain the logged date), so a new one needs to be created from the metric date up until the following cycle's start date
                var followingCycle = await _cycleRepository.GetFollowingCycle(userId, date);

                // When the following cycle contains no 'Flow', merge the new cycle with the following cycle (e.g. when the user logs a headache without logging a period)
                // Also, when the start of the new cycle is less than 2 days from the following cycle's start date, merge the new cycle with the following cycle
                if (followingCycle is not null && (followingCycle.CycleDays.All(c => c.IsPeriod == false) ||
                                                   followingCycle.StartDate - date <= TimeSpan.FromDays(2)))
                {
                    await _cycleRepository.Update(new Cycle()
                    {
                        Id = followingCycle.Id,
                        UserId = userId,
                        StartDate = date,
                        EndDate = followingCycle.EndDate
                    });

                    // If the following cycle contains any metrics, which now belong to the new cycle, update their cycle id
                    await _calendarDayRepository.UpdateCycleIds(followingCycle.Id, followingCycle.Id,
                        followingCycle.StartDate,
                        followingCycle.EndDate ?? DateTimeOffset.UtcNow);

                    cycle = followingCycle;
                } else
                {
                    cycle = await _cycleRepository.Create(new Cycle()
                    {
                        Id = Guid.NewGuid(),
                        UserId = userId,
                        StartDate = date,
                        EndDate = followingCycle?.StartDate.AddDays(-1)
                    });
                }

                var calendarDay = await GetOrCreateCalendarDay(userId, date, cycle.Id);
                if (metricNames.Values.Any(m => m.Contains("Flow")))
                {
                    await SetIsPeriodOnCalendarDay(calendarDay);
                }

                await _metricRepository.SaveMetrics(calendarDay.Id, metrics.Where(m => m.CreatedAt == date).ToList());
            }
        }
    }

    private async Task<CalendarDay> GetOrCreateCalendarDay(Guid userId, DateTimeOffset date, Guid cycleId)
    {
        var calendarDay = await _calendarDayRepository.GetByDate(userId, date);
        if (calendarDay is null)
        {
            calendarDay = await _calendarDayRepository.CreteCycleDay(userId, date, cycleId);
        }

        return calendarDay;
    }

    private async Task SetIsPeriodOnCalendarDay(CalendarDay calendarDay)
    {
        if (calendarDay is not CycleDay day)
        {
            throw new BadRequestException("Cannot log a flow metric on a non-cycle day.");
        }

        if (!day.IsPeriod)
        {
            day.IsPeriod = true;
            await _calendarDayRepository.SetIsPeriod(calendarDay.Id, true);
        }
    }

    /// <summary>
    /// Deletes a metric entry.
    /// </summary>
    /// <param name="calendarDayMetricId"></param>
    public async Task DeleteMetricEntry(Guid calendarDayMetricId)
    {
        // Keep a reference to the calendar day id before deleting the metric entry
        var calendarDayId = await _metricRepository.GetCalendarDayIdByCalendarDayMetricId(calendarDayMetricId);

        await _metricRepository.DeleteMetricEntry(calendarDayMetricId);

        // Check if there are any more metrics for the day, if not, delete the day
        var metrics = await _metricRepository.GetMetricsForCalendarDayById(calendarDayId);
        if (metrics.Count == 0)
        {
            await _calendarDayRepository.Delete(calendarDayId);
        }

        // Check if there is any 'Flow' metric left for the day, if not, set isPeriod to false
        var flowMetric = metrics.FirstOrDefault(m => m.Name.Contains("Flow"));
        if (flowMetric is null)
        {
            await _calendarDayRepository.SetIsPeriod(calendarDayId, false);
        }
    }

    private CalendarDay? BuildCalendarDayFromAdapter(CalendarDayAdapter calendarDayAdapter)
    {
        switch (calendarDayAdapter.State)
        {
            case "CycleDay":
                return new CycleDay()
                {
                    Id = calendarDayAdapter.CalendarDayId,
                    CycleId = calendarDayAdapter.CycleId!.Value,
                    Date = calendarDayAdapter.Date,
                    UserId = calendarDayAdapter.UserId,
                    State = calendarDayAdapter.State,
                    SelectedMetrics = new List<CalendarDayMetric>(),
                    IsPeriod = calendarDayAdapter.IsPeriod,
                };
            default:
                return null;
        }
    }
}
