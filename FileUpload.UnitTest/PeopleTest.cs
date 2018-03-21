using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FileUpload.Services;
using System.Collections.Generic;
using FileUpload.Models.Request;
using FileUpload.Models.Domain;

namespace ProjectExample.UnitTest
{
    [TestClass]
    public class PeopleTest
    {
        [TestMethod]
        public void Insert_Test()
        {
            // Assign
            PeopleAddRequest model = new PeopleAddRequest
            {
                FirstName = "Old",
                MiddleInitial = 'G',
                LastName = "Man",
                DOB = DateTime.Now.AddYears(-25),
                ModifiedBy = "me"
            };

            // Act
            PeopleService svc = new PeopleService();
            int result = svc.Insert(model);

            // Assert
            Assert.IsTrue(result > 0, "The insert failed!");
        }

        [TestMethod]
        public void SelectAll_Test()
        {
            PeopleService svc = new PeopleService();
            List<People> result = svc.GetAll();

            Assert.IsTrue(result.Count > 0, "Select All has failed");
        }
    }
}