using crossblog.Domain;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace crossblog.Repositories
{
    public class ArticleRepository : GenericRepository<Article>, IArticleRepository
    {
        public ArticleRepository(CrossBlogDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Article>> Search(string text)
        {
            return await _dbContext.Articles.FromSql($"SELECT * from Articles WHERE MATCH(Content) AGAINST({text}) OR MATCH(Title) AGAINST({text}) ").ToListAsync();
        }
    }
}