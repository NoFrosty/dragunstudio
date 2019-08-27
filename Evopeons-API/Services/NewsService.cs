using Dapper.Contrib.Extensions;
using Evopeons.API.Data;
using Evopeons.API.Models;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Evopeons.API.Services
{
    public class NewsService
    {
        readonly IConnectionFactory connectionFactory;

        public NewsService(IConnectionFactory connectionFactory)
        {
            this.connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<News>> GetAllNews()
        {
            using(IDbConnection connection = connectionFactory.GetConnection())
            {
                return await connection.GetAllAsync<News>();
                //return connection.Query<News>("SELECT * FROM News");
            }
        }

        public async Task AddNews(News news)
        {
            using (IDbConnection connection = connectionFactory.GetConnection())
            {
                await connection.InsertAsync(news);
                //connection.Execute("INSERT INTO News (Slug, Title, Article, PublishDate, Author) values (@Slug, @Title, @Article, @PublishDate, @Author)", news);
            }
        }
    }
}
