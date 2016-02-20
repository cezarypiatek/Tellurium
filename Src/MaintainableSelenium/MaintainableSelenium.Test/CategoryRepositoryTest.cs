using System;
using System.Collections.Generic;
using MaintainableSelenium.API;
using MaintainableSelenium.API.Models;
using MaintainableSelenium.Tests.Properties;
using NUnit.Framework;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.FileSystem;

namespace MaintainableSelenium.Tests
{
    [TestFixture]
    public class CategoryRepositoryTest
    {
        private IDocumentStore _documentStore;
        private IFilesStore _filesStore;
        private Category _category;

        [OneTimeSetUp]
        public void Init()
        {
            _documentStore = new DocumentStore { Url = "http://localhost:8080/", DefaultDatabase = "MaintainableSeleniumTestDB" }.Initialize();
            _filesStore = new FilesStore { Url = "http://localhost:8080/", DefaultFileSystem = "MaintainableSeleniumTestFS" }.Initialize();
            _category = new Category
            {
                Id = "99",
                Name = "SomeCategory",
                Browser = Browser.Chrome,
                Tests = new List<Test>
                {
                    new Test
                    {
                        Name = "Test1",
                        ExpectedImageId = Guid.NewGuid().ToString(),
                        ExpectedImage = Resources.TestImage1,
                        ResultImageId = Guid.NewGuid().ToString(),
                        ResultImage = Resources.TestImage2
                    },
                }
            };
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            _documentStore.Dispose();
            _filesStore.Dispose();
        }


        [Test]
        public void CrudTest()
        {
            using (var asyncFilesSession = _filesStore.OpenAsyncSession())
            using (var documentSession = _documentStore.OpenSession())
            {
                var target = new CategoryRepository(documentSession, asyncFilesSession);

                target.Add(_category);
                documentSession.SaveChanges();
                asyncFilesSession.SaveChangesAsync().Wait();

                var result = target.GetById(_category.Id);

                Assert.AreEqual(_category.Id, result.Id);
                Assert.AreEqual(_category.Name, result.Name);
                Assert.AreEqual(_category.Browser, result.Browser);
                Assert.AreEqual(_category.Tests[0].Name, result.Tests[0].Name);
                Assert.AreEqual(_category.Tests[0].Name, result.Tests[0].Name);
                Assert.AreEqual(_category.Tests[0].ExpectedImageId, result.Tests[0].ExpectedImageId);
                Assert.AreEqual(_category.Tests[0].ExpectedImage, result.Tests[0].ExpectedImage);
                Assert.AreEqual(_category.Tests[0].ResultImageId, result.Tests[0].ResultImageId);
                Assert.AreEqual(_category.Tests[0].ResultImage, result.Tests[0].ResultImage);
                Assert.AreEqual(1, _category.Tests.Count);

                target.Delete(result);
                documentSession.SaveChanges();
                asyncFilesSession.SaveChangesAsync().Wait();

                var deleteResult = target.GetById(_category.Id);
                Assert.IsNull(deleteResult);
            }
        }
    }
}



