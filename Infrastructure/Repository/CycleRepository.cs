using System.Data;
using Dapper;
using Infrastructure.Repository.Interface;
using Models;
using Models.Days;
using Models.Dto.Cycle;
using Models.Pagination;

namespace Infrastructure.Repository;

public class CycleRepository : ICycleRepository
{
    private readonly IDbConnection _db;

    public CycleRepository(IDbConnection db)
    {
        _db = db;
    }

    public async Task<PaginatedList<Cycle>> Get(Paginator paginator)
    {
        var sql = @"SELECT * FROM ""Cycles""
                        LIMIT @PageSize OFFSET @Offset";
        var count = _db.QuerySingle<int>(@"SELECT COUNT(*) FROM ""Cycles""");
        var list = await _db.QueryAsync<Cycle>(sql, new
        {
            PageSize = paginator.ItemsPerPage,
            Offset = (paginator.Page - 1) * paginator.ItemsPerPage
        });

        var enumerable = list.ToList();
        enumerable.ToList().ForEach(cycle =>
        {
            cycle.CycleDays = GetCycleDaysForCycleAsync(cycle.Id).Result.ToList();
        });

        return await PaginatedList<Cycle>.CreateAsync(enumerable, paginator.Page, paginator.ItemsPerPage, count);
    }

    public async Task<Cycle?> GetById(Guid id)
    {
        var sql = @"SELECT * FROM ""Cycles"" WHERE ""Id""=@id";
        var cycle = await _db.QuerySingleOrDefaultAsync<Cycle>(sql, new { id });
        if (cycle != null) cycle.CycleDays = GetCycleDaysForCycleAsync(cycle.Id).Result.ToList();
        return cycle;
    }

    /// <summary>
    /// Retrieves a list of period and cycle lengths for the specified user for a number of cycles.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="numberOfCycles"></param>
    /// <returns></returns>
    public async Task<List<PeriodAndCycleLengthDto>> GetPeriodAndCycleLengths(Guid userId, int numberOfCycles)
    {
        //get list of last number of cycles and their cycle days where enddate is today or later
        var cycleList = await GetRecentCyclesWithDays(userId, numberOfCycles);

        //get difference between start and end dates of each cycle
        var periodAndCycleLengths = cycleList.Select(cycle =>
        {
            var cycleLength = cycle.EndDate - cycle.StartDate;
            var periodLength = cycle.CycleDays.Count(cd => cd.IsPeriod);
            return new PeriodAndCycleLengthDto
            {
                CycleLength = (float)cycleLength?.Days,
                PeriodLength = periodLength
            };
        });
        return periodAndCycleLengths.ToList();
    }

    /// <summary>
    /// Retrieves a list of the most recent of cycles for the specified user.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="numberOfCycles"></param>
    /// <returns></returns>
    public async Task<List<Cycle>> GetRecentCyclesWithDays(Guid userId, int numberOfCycles)
    {
        var sql =
            @"SELECT * FROM ""Cycles"" WHERE ""UserId""=@UserId AND ""EndDate"" IS NOT NULL ORDER BY ""StartDate"" DESC LIMIT @NumberOfCycles";
        var cycles = await _db.QueryAsync<Cycle>(sql, new { UserId = userId, NumberOfCycles = numberOfCycles });
        var cycleList = cycles.ToList();
        cycleList.ForEach(cycle => { cycle.CycleDays = GetCycleDaysForCycleAsync(cycle.Id).Result.ToList(); });
        return cycleList;
    }

    public async Task<Cycle> Create(Cycle cycle)
    {
        cycle.StartDate = cycle.StartDate.ToOffset(TimeSpan.Zero);
        cycle.EndDate = cycle.EndDate?.ToOffset(TimeSpan.Zero);
        var sql = @"INSERT INTO ""Cycles"" (""Id"", ""StartDate"", ""EndDate"", ""UserId"") 
                VALUES (@Id, @StartDate, @EndDate, @UserId)";
        await _db.ExecuteAsync(sql, cycle);
        return cycle;
    }

    public async Task<Cycle> Update(Cycle cycle)
    {
        cycle.StartDate = cycle.StartDate.ToOffset(TimeSpan.Zero);
        cycle.EndDate = cycle.EndDate?.ToOffset(TimeSpan.Zero);
        var sql = @"UPDATE ""Cycles"" SET ""StartDate""=@StartDate, ""EndDate""=@EndDate WHERE ""Id""=@Id";

        await _db.ExecuteAsync(sql, new
        {
            cycle.Id,
            cycle.StartDate,
            cycle.EndDate
        });

        return cycle;
    }

    private async Task<IEnumerable<CycleDay>> GetCycleDaysForCycleAsync(Guid cycleId)
    {
        var sql = @"SELECT * FROM ""CalendarDay"" WHERE ""CycleId""=@CycleId";
        var cycleDays = await _db.QueryAsync<CycleDay>(sql, new { CycleId = cycleId });
        return cycleDays;
    }

    public Task<Cycle?> GetCurrentCycle(Guid userId)
    {
        var sql = @"SELECT * FROM ""Cycles"" WHERE ""UserId""=@UserId AND ""EndDate"" IS NULL";
        return _db.QuerySingleOrDefaultAsync<Cycle>(sql, new { UserId = userId });
    }

    public Task<Cycle?> GetCycleByDate(Guid userId, DateTimeOffset date)
    {
        var sql =
            @"SELECT * FROM ""Cycles"" WHERE ""UserId""=@UserId AND CAST(""StartDate"" AS DATE) <= CAST(@Date AS DATE) AND CAST(""EndDate"" AS DATE) >= CAST(@Date as DATE)";
        return _db.QuerySingleOrDefaultAsync<Cycle>(sql, new { UserId = userId, Date = date });
    }
    
    public Task<Cycle> GetFollowingCycle(Guid userId, DateTimeOffset date)
    {
        var sql =
            @"SELECT * FROM ""Cycles"" WHERE ""UserId""=@UserId AND CAST(""StartDate"" AS DATE) > CAST(@Date AS DATE) ORDER BY ""StartDate"" LIMIT 1";
        return _db.QuerySingleOrDefaultAsync<Cycle>(sql, new { UserId = userId, Date = date });
    }
}
