using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Evopeons.API.Articles
{
    public class ArticleManager
    {
        private readonly ArticleDataAccess articleDataAccess;
        public ArticleManager(ArticleDataAccess articleDataAccess)
        {
            this.articleDataAccess = articleDataAccess;
        }

        public async Task<IEnumerable<Article>> GetAllArticles(bool isPublished)
        {
            return await articleDataAccess.GetAllArticles(isPublished);
        }

        public async Task<Article> GetArticle(Guid slug)
        {
            return await articleDataAccess.GetArticle(slug);
        }

        public async Task AddArticle(Article article)
        {
            await articleDataAccess.AddArticle(article);
        }

        public async void DeleteArticle(string slug)
        {
           await articleDataAccess.DeleteArticle(slug);
        }

        public async Task<IEnumerable<Article>> GetAllArticleByApplication(Guid applicationId, bool isPublished)
        {
            return await articleDataAccess.GetAllArticlesByApplication(applicationId, isPublished);
        }

        
    }
}
