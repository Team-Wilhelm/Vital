using AutoMapper;
using Models;
using Models.Dto.Cycle;
using Models.Dto.Metrics;
using Models.Pagination;

namespace Vital.Extension.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        //Cycle
        CreateMap<Cycle, CycleDto>();
        CreateMap<CreateCycleDto, Cycle>();
        CreateMap<UpdateCycleDto, Cycle>();
        CreateMap<PaginatedList<Cycle>, PaginatedList<CycleDto>>();


        // Metric
        CreateMap<Metrics, MetricsDto>();
        CreateMap<MetricValue, MetricValueDto>();
        CreateMap<CalendarDayMetric, CalendarDayMetricDto>();
        CreateMap<MetricValue, ValueDto>();
    }
}
