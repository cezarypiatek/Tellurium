using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using MaintainableSelenium.API.Models;
using Raven.Client;
using Raven.Client.FileSystem;

namespace MaintainableSelenium.API
{
    public class CategoryRepository : IRepository<Category>
    {
        private readonly IDocumentSession _documentSession;
        private readonly IAsyncFilesSession _asyncFilesSession;

        public CategoryRepository(IDocumentSession documentSession, IAsyncFilesSession asyncFilesSession)
        {
            _documentSession = documentSession;
            _asyncFilesSession = asyncFilesSession;
        }

        public IEnumerable<Category> GetAll()
        {
            var categories = _documentSession.Query<Category>();
            foreach (var category in categories)
            {
                LoadTestImages(category.Tests, _asyncFilesSession);
            }

            return categories;
        }

        public Category GetById(string id)
        {
            var category = _documentSession.Load<Category>(id);
            if (category != null)
                LoadTestImages(category.Tests, _asyncFilesSession);

            return category;
        }


        public void Add(Category entity)
        {
            SaveTestImages(entity.Tests, _asyncFilesSession);
            _documentSession.Store(entity);
        }

        public void Update(Category entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(Category entity)
        {
            RemoveTestImages(entity.Tests, _asyncFilesSession);

            _documentSession.Delete(entity);
        }

        private void LoadTestImages(IEnumerable<Test> tests, IAsyncFilesSession filesSession)
        {
            foreach (var test in tests)
            {
                if (!string.IsNullOrEmpty(test.ExpectedImageId))
                    test.ExpectedImage = Image.FromStream(filesSession.DownloadAsync(test.ExpectedImageId).Result);

                if (!string.IsNullOrEmpty(test.ResultImageId))
                    test.ExpectedImage = Image.FromStream(filesSession.DownloadAsync(test.ResultImageId).Result);
            }
        }

        private void SaveTestImages(IEnumerable<Test> tests, IAsyncFilesSession filesSession)
        {
            foreach (var test in tests)
            {
                if (!string.IsNullOrEmpty(test.ExpectedImageId))
                    filesSession.RegisterUpload(test.ExpectedImageId, test.ExpectedImage.ToStream(ImageFormat.Jpeg));

                if (!string.IsNullOrEmpty(test.ResultImageId))
                    filesSession.RegisterUpload(test.ResultImageId, test.ResultImage.ToStream(ImageFormat.Jpeg));
            }
        }

        private void RemoveTestImages(IEnumerable<Test> tests, IAsyncFilesSession filesSession)
        {
            foreach (var test in tests)
            {
                if (!string.IsNullOrEmpty(test.ExpectedImageId))
                    filesSession.RegisterFileDeletion(test.ExpectedImageId);

                if (!string.IsNullOrEmpty(test.ResultImageId))
                    filesSession.RegisterFileDeletion(test.ResultImageId);
            }
        }
    }
}

 