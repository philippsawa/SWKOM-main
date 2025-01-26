using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWKOM_SAWA_KIM.ElasticSearch
{
    public interface ISearchIndex
    {
        void AddDocumentAsync(SearchDocumentEntity document);
        IEnumerable<SearchDocumentEntity> SearchDocumentAsync(string query);
    }
}
