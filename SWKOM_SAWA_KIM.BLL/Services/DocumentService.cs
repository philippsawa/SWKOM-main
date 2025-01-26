using AutoMapper;
using Microsoft.Extensions.Logging;
using SWKOM_SAWA_KIM.BLL.DTOs;
using SWKOM_SAWA_KIM.DAL.Entities;
using SWKOM_SAWA_KIM.DAL.Repositories;
using SWKOM_SAWA_KIM.ElasticSearch;
using SWKOM_SAWA_KIM.RabbitMQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWKOM_SAWA_KIM.BLL.Services
{ 
    public class DocumentService : IDocumentService
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly IMapper _mapper;
        private readonly ISearchIndex _searchIndex;

        public DocumentService(IDocumentRepository documentRepository, IMapper mapper, ISearchIndex searchIndex)
        {
            _documentRepository = documentRepository;
            _mapper = mapper;
            _searchIndex = searchIndex;
        }

        public async Task<IEnumerable<DocumentDTO>> GetAllDocumentsAsync()
        {
            var documents = await _documentRepository.GetAllDocumentsAsync();
            var documentDTOs = _mapper.Map<IEnumerable<DocumentDTO>>(documents);
            return documentDTOs;
        }

        public async Task<DocumentDTO> GetDocumentByIdAsync(string id)
        {
            var document = await _documentRepository.GetDocumentByIdAsync(id);
            if (document == null)
            {
                // throw exception
                return null;
            }

            var documentDTO = _mapper.Map<DocumentDTO>(document);
            return documentDTO;
        }

        public async Task AddDocumentAsync(DocumentDTO documentDTO)
        {
            var document = _mapper.Map<Document>(documentDTO);
            await _documentRepository.AddDocumentAsync(document);
        }

        public async Task UpdateDocumentAsync(string id, string content)
        {
            var document = await _documentRepository.GetDocumentByIdAsync(id);

            if(document == null)
            {
                // throw exception
                return;
            }

            document.Content = content;

            await _documentRepository.UpdateDocumentAsync(document);
        }

        public IEnumerable<SearchDocumentEntity> SearchDocuments(string query)
        {
            return _searchIndex.SearchDocumentAsync(query);
        }

        public async Task DeleteDocumentAsync(string id)
        {
            await _documentRepository.DeleteDocumentAsync(id);
        }
    }
}
