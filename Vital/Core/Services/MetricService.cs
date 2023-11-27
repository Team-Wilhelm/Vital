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
                calendarDay = BuildCalendarDay(calendarDayAdapter);
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
            
            MetricValue metricValue;
            if (calendarDayAdapter.MetricValueId is not null)
            {
                metricValue= new MetricValue()
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

    public async Task<CalendarDayDto> SaveMetrics(Guid userId, List<MetricRegisterMetricDto> metrics,
        DateTimeOffset dateTimeOffset)
    {
        var calendarDay = await _calendarDayRepository.GetByDate(userId, dateTimeOffset);
        if (calendarDay is null)
        {
            calendarDay = await _calendarDayRepository.CreteCycleDay(userId, dateTimeOffset);
        }

        await _metricRepository.SaveMetrics(calendarDay.Id, metrics);
        calendarDay = await _calendarDayRepository.GetById(calendarDay.Id); //TODO: Include a list of selected metrics
        return _mapper.Map<CalendarDayDto>(calendarDay);
    }
    
    private CalendarDay? BuildCalendarDay(CalendarDayAdapter calendarDayAdapter)
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
