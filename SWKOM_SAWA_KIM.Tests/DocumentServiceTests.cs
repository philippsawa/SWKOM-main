using Moq;
using SWKOM_SAWA_KIM.BLL.DTOs;
using SWKOM_SAWA_KIM.BLL.Services;
using SWKOM_SAWA_KIM.DAL.Entities;
using SWKOM_SAWA_KIM.DAL.Repositories;
using SWKOM_SAWA_KIM.ElasticSearch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using SWKOM_SAWA_KIM.BLL.Mappings;
using SWKOM_SAWA_KIM.BLL.Validators;

namespace SWKOM_SAWA_KIM.Tests
{
    internal class DocumentServiceTests
    {
        [Test]
        public async Task GetDocumentByIdAsync_ShouldReturnDocument_WhenDocumentExists()
        {
            // Arrange
            var documentId = "123";
            var document = new Document { Id = documentId, Filename = "test.pdf", Content = "test content" };
            var documentDTO = new DocumentDTO { Id = documentId, Filename = "test.pdf", Content = "test content" };

            var mockRepository = new Mock<IDocumentRepository>();
            mockRepository.Setup(repo => repo.GetDocumentByIdAsync(documentId))
                          .ReturnsAsync(document);

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(mapper => mapper.Map<DocumentDTO>(document))
                      .Returns(documentDTO);

            var service = new DocumentService(mockRepository.Object, mockMapper.Object, Mock.Of<ISearchIndex>());

            // Act
            var result = await service.GetDocumentByIdAsync(documentId);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(documentDTO, result);
        }

        [Test]
        public void MappingProfile_ShouldMapDocumentToDocumentDTO()
        {
            // Arrange
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            var mapper = config.CreateMapper();

            var document = new Document
            {
                Id = "123",
                Filename = "test.pdf",
                Content = "Sample content",
                CreatedAt = DateTime.UtcNow
            };

            // Act
            var documentDTO = mapper.Map<DocumentDTO>(document);

            // Assert
            Assert.AreEqual(document.Id, documentDTO.Id);
            Assert.AreEqual(document.Filename, documentDTO.Filename);
            Assert.AreEqual(document.Content, documentDTO.Content);
            Assert.AreEqual(document.CreatedAt, documentDTO.CreatedAt);
        }

        [Test]
        public void Validate_ShouldFail_WhenFilenameIsInvalid()
        {
            // Arrange
            var validator = new DocumentDTOValidator();
            var dto = new DocumentDTO { Filename = "invalid.txt" };

            // Act
            var result = validator.Validate(dto);

            // Assert
            Assert.False(result.IsValid);
            Assert.AreEqual("Filename must end with .pdf", result.Errors.First().ErrorMessage);
        }

        [Test]
        public async Task GetAllDocumentsAsync_ShouldReturnAllDocuments()
        {
            // Arrange
            var documents = new List<Document>
            {
                new Document { Id = "1", Filename = "File1.pdf", Content = "Content1" },
                new Document { Id = "2", Filename = "File2.pdf", Content = "Content2" }
            };

            var documentDTOs = new List<DocumentDTO>
            {
                new DocumentDTO { Id = "1", Filename = "File1.pdf", Content = "Content1" },
                new DocumentDTO { Id = "2", Filename = "File2.pdf", Content = "Content2" }
            };

            var mockRepository = new Mock<IDocumentRepository>();
            mockRepository.Setup(repo => repo.GetAllDocumentsAsync())
                          .ReturnsAsync(documents);

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(mapper => mapper.Map<IEnumerable<DocumentDTO>>(documents))
                      .Returns(documentDTOs);

            var service = new DocumentService(mockRepository.Object, mockMapper.Object, Mock.Of<ISearchIndex>());

            // Act
            var result = await service.GetAllDocumentsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual("File1.pdf", result.First().Filename);
        }

        [Test]
        public async Task AddDocumentAsync_ShouldMapAndSaveDocument()
        {
            // Arrange
            var documentDTO = new DocumentDTO { Id = "123", Filename = "test.pdf", Content = "test content" };
            var document = new Document { Id = "123", Filename = "test.pdf", Content = "test content" };

            var mockRepository = new Mock<IDocumentRepository>();
            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(mapper => mapper.Map<Document>(documentDTO)).Returns(document);

            var service = new DocumentService(mockRepository.Object, mockMapper.Object, Mock.Of<ISearchIndex>());

            // Act
            await service.AddDocumentAsync(documentDTO);

            // Assert
            mockMapper.Verify(mapper => mapper.Map<Document>(documentDTO), Times.Once);
            mockRepository.Verify(repo => repo.AddDocumentAsync(document), Times.Once);
        }

        [Test]
        public async Task UpdateDocumentAsync_ShouldUpdateDocumentContent()
        {
            // Arrange
            var documentId = "123";
            var existingDocument = new Document { Id = documentId, Filename = "test.pdf", Content = "Old content" };

            var mockRepository = new Mock<IDocumentRepository>();
            mockRepository.Setup(repo => repo.GetDocumentByIdAsync(documentId))
                          .ReturnsAsync(existingDocument);

            var service = new DocumentService(mockRepository.Object, Mock.Of<IMapper>(), Mock.Of<ISearchIndex>());

            // Act
            await service.UpdateDocumentAsync(documentId, "New content");

            // Assert
            Assert.AreEqual("New content", existingDocument.Content);
            mockRepository.Verify(repo => repo.UpdateDocumentAsync(existingDocument), Times.Once);
        }

        [Test]
        public void SearchDocuments_ShouldReturnSearchResults()
        {
            // Arrange
            var query = "test";
            var searchResults = new List<SearchDocumentEntity>
            {
                new SearchDocumentEntity { Id = "1", Filename = "File1.pdf", Content = "test content 1" },
                new SearchDocumentEntity { Id = "2", Filename = "File2.pdf", Content = "test content 2" }
            };

            var mockSearchIndex = new Mock<ISearchIndex>();
            mockSearchIndex.Setup(index => index.SearchDocumentAsync(query))
                           .Returns(searchResults);

            var service = new DocumentService(Mock.Of<IDocumentRepository>(), Mock.Of<IMapper>(), mockSearchIndex.Object);

            // Act
            var result = service.SearchDocuments(query);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(2, result.Count());
            Assert.IsTrue(result.Any(d => d.Filename == "File1.pdf"));
        }
    }
}
