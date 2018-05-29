using crossblog.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace crossblog.Repositories
{
    public interface IArticleRepository : IGenericRepository<Article>
    {
        Task<IEnumerable<Article>> Search(string text);
    }
}