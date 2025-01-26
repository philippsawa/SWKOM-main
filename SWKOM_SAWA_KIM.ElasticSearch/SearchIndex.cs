using Elastic.Clients.Elasticsearch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace SWKOM_SAWA_KIM.ElasticSearch
{
    public class SearchIndex : ISearchIndex
    {
        private readonly Uri _uri;

        public SearchIndex(IConfiguration config)
        {
            var url = config.GetSection("ElasticSearchOptions:Url").Value;
            _uri = new Uri(url);
        }

        public void AddDocumentAsync(SearchDocumentEntity document)
        {
            var elasticClient = new ElasticsearchClient(_uri);

            if(!elasticClient.Indices.Exists("documents").Exists)
                elasticClient.Indices.Create("documents");

            var indexResponse = elasticClient.Index(document, "documents");
            if (!indexResponse.IsSuccess())
            {
                return;
            }
        }

        public IEnumerable<SearchDocumentEntity> SearchDocumentAsync(string query)
        {
            var elasticClient = new ElasticsearchClient(_uri);

            var searchResponse = elasticClient.Search<SearchDocumentEntity>(s => s
                .Index("documents")
                .Query(q => q.QueryString(qs => qs.DefaultField(p => p.Content).Query($"*{query}*")))
            );

            return searchResponse.Documents;
        }
    }
}
