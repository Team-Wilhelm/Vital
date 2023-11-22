using AutoMapper;
using Infrastructure.Repository.Interface;
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

    public async Task<ICollection<CalendarDayMetric>> Get(Guid userId, DateTimeOffset date)
    {
        var list = await _metricRepository.Get(userId, date);
        return list;
    }

    public async Task<CalendarDayDto> UploadMetricForADay(Guid userId, List<MetricRegisterMetricDto> metrics,
        DateTimeOffset dateTimeOffset)
    {
        var calendarDay = await _calendarDayRepository.GetByDate(userId, dateTimeOffset);
        if (calendarDay is null)
        {
            throw new NotFoundException();
        }

        await _metricRepository.UploadMetricForADay(calendarDay.Id, metrics);
        calendarDay = await _calendarDayRepository.GetById(calendarDay.Id); //TODO: Include a list of selected metrics
        return _mapper.Map<CalendarDayDto>(calendarDay);
    }
}
