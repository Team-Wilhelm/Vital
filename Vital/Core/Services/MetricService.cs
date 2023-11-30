using System.Globalization;
using AutoMapper;
using Infrastructure.Adapters;
using Infrastructure.Repository.Interface;
using Models;
using Models.Days;
using Models.Dto.Cycle;
using Models.Dto.Metrics;
using Models.Util;
using Vital.Core.Services.Interfaces;
using Vital.Models.Exception;

namespace Vital.Core.Services;

public class MetricService : IMetricService
{
    private readonly IMetricRepository _metricRepository;
    private readonly ICalendarDayRepository _calendarDayRepository;
    private readonly IMapper _mapper;

    public MetricService(IMetricRepository metricRepository, ICalendarDayRepository calendarDayRepository, IMapper mapper)
    {
        _metricRepository = metricRepository;
        _calendarDayRepository = calendarDayRepository;
        _mapper = mapper;
    }

    public async Task<List<Metrics>> GetAllMetrics()
    {
        return await _metricRepository.GetAllMetrics();
    }
    
    public async Task<IEnumerable<CalendarDay>> GetMetricsForCalendarDays(Guid userId, DateTimeOffset fromDate, DateTimeOffset toDate)
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
                calendarDays.Add(calendarDay);
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
                Values = new List<MetricValue>()
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
            calendarDay.SelectedMetrics.Add(calendarDayMetric);
        }

        return calendarDays;
    }

    public async Task<ICollection<CalendarDayMetric>> Get(Guid userId, DateTimeOffset date)
    {
        var list = await _metricRepository.Get(userId, date);
        return list;
    }
    
    public async Task<IEnumerable<DateTimeOffset>> GetPeriodDays(Guid userId, DateTimeOffset fromDate, DateTimeOffset toDate)
    {
        var list = await _metricRepository.GetPeriodDays(userId, fromDate, toDate);
        return list;
    }

    public async Task SaveMetrics(Guid userId, List<MetricRegisterMetricDto> metrics)
    {
        var dayList = metrics.Select(m => m.CreatedAt).Distinct().ToList();
        foreach (var date in dayList)
        {
            var calendarDay = await _calendarDayRepository.GetByDate(userId, date) ?? await _calendarDayRepository.CreteCycleDay(userId, date);
            
            // Check one of the metrics we're saving is a flow metric and if so, update isPeriod on the calendar day
            var metricNames = await _metricRepository.GetMetricNamesByIds(metrics.Select(m => m.MetricsId).ToList()); // Get the metric names for the metrics we're saving
            
            // Check if the metric names contain "Flow" and if so, update the calendar day to be a period day
            if (metricNames.Values.Any(m => m.Contains("Flow")))
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
            await _metricRepository.SaveMetrics(calendarDay.Id, metrics.Where(m => m.CreatedAt == date).ToList());
        }
    }

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
