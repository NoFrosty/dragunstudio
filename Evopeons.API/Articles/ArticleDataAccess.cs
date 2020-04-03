using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using Evorine.EvoPackageManager;

namespace Evopeons.API.Articles
{
    public class ArticleDataAccess
    {
        readonly ConnectionFactory connectionFactory;

        public ArticleDataAccess(ConnectionFactory connectionFactory)
        {
            this.connectionFactory = connectionFactory;
        }
        public async Task<IEnumerable<Article>> GetAllArticles(bool includeNotPublished)
        {
            using (var connection = connectionFactory.AcquireConnection())
            {
                string sql = includeNotPublished ? "SELECT * FROM Article" : "SELECT * FROM Article WHERE Published = 1";
                return await connection.QueryAsync<Article>(sql);
            }
        }

        public async Task<IEnumerable<Article>> GetAllArticlesByApplication(Guid applicationId, bool isPublished)
        {
            using (var connection = connectionFactory.AcquireConnection())
            {
                string sql = isPublished ? "SELECT * FROM Article WHERE ApplicationId = @ApplicationId" : "SELECT * FROM Article WHERE ApplicationId = @ApplicationId AND Published = 1";
                return await connection.QueryAsync<Article>(
                    sql,
                    new { ApplicationId = applicationId }
                );
            }
        }

        public async Task<Article> GetArticle(Guid slug)
        {
            using (var connection = connectionFactory.AcquireConnection())
            {
                return await connection.QueryFirstAsync<Article>(
                    "SELECT * FROM Article WHERE Slug = @Slug",
                    new { Slug = slug }
                );
            }
        }

        public async Task AddArticle(Article article)
        {
            using (var connection = connectionFactory.AcquireConnection())
            {
                await connection.InsertAsync(article);
            }
        }

        public async Task UpdateArticle(Article article)
        {
            using (var connection = connectionFactory.AcquireConnection())
            {
                await connection.UpdateAsync(article);
            }
        }

        public async Task DeleteArticle(Article article)
        {
            using (var connection = connectionFactory.AcquireConnection())
            {
                await connection.DeleteAsync(article);
            }
        }

        public async Task DeleteArticle(string slug)
        {
            using (var connection = connectionFactory.AcquireConnection())
            {
                await connection.ExecuteAsync(
                    "DELETE FROM Article WHERE Slug = @Slug",
                    new { Slug = slug }
                ); ;
            }
        }
    }
}
