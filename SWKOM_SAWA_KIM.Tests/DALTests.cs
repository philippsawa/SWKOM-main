using SWKOM_SAWA_KIM.DAL.Entities;
using SWKOM_SAWA_KIM.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Microsoft.EntityFrameworkCore.InMemory;
using Microsoft.EntityFrameworkCore;
using SWKOM_SAWA_KIM.DAL.Data;

namespace SWKOM_SAWA_KIM.Tests
{
    internal class DALTests
    {
        private ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }


        [Test]
        public async Task GetAllDocumentsAsync_ShouldReturnAllDocuments()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            var repository = new DocumentRepository(dbContext);

            dbContext.Documents.AddRange(new List<Document>
            {
                new Document { Id = "1", Filename = "File1.pdf", Content = "Content1" },
                new Document { Id = "2", Filename = "File2.pdf", Content = "Content2" }
            });
            await dbContext.SaveChangesAsync();

            // Act
            var result = await repository.GetAllDocumentsAsync();

            // Assert
            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public async Task GetDocumentByIdAsync_ShouldReturnDocument_WhenDocumentExists()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            var repository = new DocumentRepository(dbContext);

            var document = new Document { Id = "1", Filename = "File1.pdf", Content = "Content1" };
            dbContext.Documents.Add(document);
            await dbContext.SaveChangesAsync();

            // Act
            var result = await repository.GetDocumentByIdAsync("1");

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual("File1.pdf", result.Filename);
        }

        [Test]
        public async Task AddDocumentAsync_ShouldAddDocumentToDatabase()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            var repository = new DocumentRepository(dbContext);

            var document = new Document { Id = "1", Filename = "File1.pdf", Content = "Content1" };

            // Act
            await repository.AddDocumentAsync(document);

            // Assert
            Assert.AreEqual(1, dbContext.Documents.Count());
            Assert.AreEqual("File1.pdf", dbContext.Documents.First().Filename);
        }

        [Test]
        public async Task UpdateDocumentAsync_ShouldUpdateDocumentContent()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            var repository = new DocumentRepository(dbContext);

            var document = new Document { Id = "1", Filename = "File1.pdf", Content = "OldContent" };
            dbContext.Documents.Add(document);
            await dbContext.SaveChangesAsync();

            // Act
            document.Content = "NewContent";
            await repository.UpdateDocumentAsync(document);

            // Assert
            Assert.AreEqual("NewContent", dbContext.Documents.First().Content);
        }

        [Test]
        public async Task DeleteDocumentAsync_ShouldRemoveDocument_WhenDocumentExists()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            var repository = new DocumentRepository(dbContext);

            var document = new Document { Id = "1", Filename = "File1.pdf" };
            dbContext.Documents.Add(document);
            await dbContext.SaveChangesAsync();

            // Act
            await repository.DeleteDocumentAsync("1");

            // Assert
            Assert.AreEqual(0, dbContext.Documents.Count());
        }

        [Test]
        public async Task GetDocumentByIdAsync_ShouldReturnNull_WhenDocumentDoesNotExist()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            var repository = new DocumentRepository(dbContext);

            // Act
            var result = await repository.GetDocumentByIdAsync("NonExistentId");

            // Assert
            Assert.Null(result);
        }

        [Test]
        public async Task UpdateDocumentAsync_ShouldNotThrow_WhenDocumentDoesNotExist()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            var repository = new DocumentRepository(dbContext);

            var nonExistentDocument = new Document { Id = "NonExistentId", Filename = "File1.pdf", Content = "Content1" };

            // Act & Assert
            Assert.DoesNotThrowAsync(() => repository.UpdateDocumentAsync(nonExistentDocument));
        }

        [Test]
        public async Task AddDocumentAsync_ShouldAddMultipleDocumentsToDatabase()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            var repository = new DocumentRepository(dbContext);

            var documents = new List<Document>
            {
                new Document { Id = "1", Filename = "File1.pdf", Content = "Content1" },
                new Document { Id = "2", Filename = "File2.pdf", Content = "Content2" }
            };

            // Act
            foreach (var document in documents)
            {
                await repository.AddDocumentAsync(document);
            }

            // Assert
            Assert.AreEqual(2, dbContext.Documents.Count());
            Assert.IsTrue(dbContext.Documents.Any(d => d.Filename == "File1.pdf"));
            Assert.IsTrue(dbContext.Documents.Any(d => d.Filename == "File2.pdf"));
        }

        [Test]
        public async Task GetAllDocumentsAsync_ShouldReturnEmptyList_WhenNoDocumentsExist()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            var repository = new DocumentRepository(dbContext);

            // Act
            var result = await repository.GetAllDocumentsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(0, result.Count());
        }
    }
}
