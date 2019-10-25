using Dapper;
using Dapper.Contrib.Extensions;
using Evopeons.API.Data;
using Evopeons.API.Models;
using System;
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
            }
        }

        public async Task<News> GetNews(string slug)
        {
            using (IDbConnection connection = connectionFactory.GetConnection())
            {
                return await connection.QueryFirstAsync<News>("SELECT * FROM News WHERE slug = '" + slug + "'");
            }
        }

        public async Task<IEnumerable<News>> GetLastXNews(int nbNews)
        {
            using (IDbConnection connection = connectionFactory.GetConnection())
            {
                return await connection.QueryAsync<News>("SELECT * FROM News ORDER BY Id DESC LIMIT " + nbNews);
            }
        }

        public async Task AddNews(News news)
        {
            using (IDbConnection connection = connectionFactory.GetConnection())
            {
                await connection.InsertAsync(news);
            }
        }

        public async Task<bool> UpdateNews(News news)
        {
            using (IDbConnection connection = connectionFactory.GetConnection())
            {
                return await connection.UpdateAsync<News>(news);
            }
        }

        public async Task<bool> DeleteNews(News news)
        {
            using (IDbConnection connection = connectionFactory.GetConnection())
            {
                return await connection.DeleteAsync<News>(news);
            }
        }

        public async Task<int> DeleteNews(int id)
        {
            using (IDbConnection connection = connectionFactory.GetConnection())
            {
                return await connection.ExecuteAsync("DELETE FROM News WHERE Id='" + id + "'");
            }
        }

        public async Task<int> DeleteNews(string slug)
        {
            using (IDbConnection connection = connectionFactory.GetConnection())
            {
                return await connection.ExecuteAsync("DELETE FROM News WHERE Slug='" + slug + "'");
            }
        }
    }
}
