using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SWKOM_SAWA_KIM.DAL.Data;
using SWKOM_SAWA_KIM.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWKOM_SAWA_KIM.DAL.Repositories
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly ApplicationDbContext _context;

        public DocumentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Document>> GetAllDocumentsAsync()
        {
            return await _context.Documents.ToListAsync();
        }

        public async Task<Document> GetDocumentByIdAsync(string id)
        {
            return await _context.Documents.FindAsync(id);
        }

        public async Task AddDocumentAsync(Document document)
        {
            await _context.Documents.AddAsync(document);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateDocumentAsync(Document document)
        {
            await _context.SaveChangesAsync();
        }

        public async Task DeleteDocumentAsync(string id)
        {
            var document = await _context.Documents.FindAsync(id);
            if (document == null)
            {
                return;
            }

            _context.Documents.Remove(document);
            await _context.SaveChangesAsync();
        }
    }
}
