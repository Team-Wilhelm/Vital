using System.Data;
using Dapper;
using Infrastructure.Repository.Interface;
using Models;
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
        return await PaginatedList<Cycle>.CreateAsync(list, paginator.Page, paginator.ItemsPerPage, count);
    }

    public async Task<Cycle?> GetById(Guid id)
    {
        var sql = @"SELECT * FROM Cycles WHERE Id=@id";
        return await _db.QuerySingleOrDefaultAsync<Cycle>(sql, new {id});
    }
    
}
