using Evopeons.API.Data;
using System;
using System.Collections.Generic;
using System.Linq;
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



    }
}
