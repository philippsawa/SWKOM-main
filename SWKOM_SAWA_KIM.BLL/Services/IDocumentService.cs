using SWKOM_SAWA_KIM.BLL.DTOs;
using SWKOM_SAWA_KIM.DAL.Entities;
using SWKOM_SAWA_KIM.ElasticSearch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWKOM_SAWA_KIM.BLL.Services
{
    public interface IDocumentService
    {
        Task<IEnumerable<DocumentDTO>> GetAllDocumentsAsync();
        Task<DocumentDTO> GetDocumentByIdAsync(string id);
        Task AddDocumentAsync(DocumentDTO document);
        Task UpdateDocumentAsync(string id, string content);
        Task DeleteDocumentAsync(string id);
        IEnumerable<SearchDocumentEntity> SearchDocuments(string query);
    }
}
