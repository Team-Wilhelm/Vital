using System.Data;
using Dapper;
using Infrastructure.Repository.Interface;
using Models;
using Models.Days;
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

    public async Task<Cycle> Create(Cycle cycle)
    {
        var sql = @"INSERT INTO ""Cycles"" (""Id"", ""StartDate"", ""EndDate"", ""UserId"") 
                VALUES (@Id, @StartDate, @EndDate, @UserId)";

        await _db.ExecuteAsync(sql, cycle);
        return cycle;
    }

    public async Task<Cycle> Update(Cycle cycle)
    {
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
}
