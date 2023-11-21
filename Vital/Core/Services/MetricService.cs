using AutoMapper;
using Infrastructure.Repository.Interface;
using Models;
using Models.Dto.Cycle;
using Models.Dto.Metrics;
using Models.Util;
using Vital.Core.Services.Interfaces;
using Vital.Models.Exception;

namespace Vital.Core.Services;

public class MetricService : IMetricService
{
    //TODO: Add interface
    private readonly IMetricRepository _metricRepository;
    private readonly ICalendarDayRepository _calendarDayRepository;
    private readonly IMapper _mapper;
    
    public MetricService(IMetricRepository metricRepository, ICalendarDayRepository calendarDayRepository, IMapper mapper)
    {
        _metricRepository = metricRepository;
        _calendarDayRepository = calendarDayRepository;
        _mapper = mapper;
    }

    public async Task<ICollection<CalendarDayMetric>> Get(Guid userId, DateTimeOffset date)
    {
        var list = await _metricRepository.Get(userId, date);
        return list;
    }

    public async Task<CalendarDayDto> UploadMetricForADay(Guid userId, List<MetricsDto> metricsDtoList,
        DateTimeOffset dateTimeOffset)
    {
        var calendarDay = await _calendarDayRepository.GetByDate(dateTimeOffset, userId);
        if (calendarDay is null)
        {
            throw new NotFoundException();
        }
        
        await _metricRepository.UploadMetricForADay(calendarDay.Id, metricsDtoList);
        calendarDay = await _calendarDayRepository.GetById(calendarDay.Id);
        return _mapper.Map<CalendarDayDto>(calendarDay);
    }
}
