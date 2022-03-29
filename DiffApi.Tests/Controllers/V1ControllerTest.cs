using DiffApi.Controllers;
using DiffApi.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using System.Web.Http.Results;

namespace DiffApi.Tests.Controllers
{
    [TestClass]
    public class V1ControllerTest
    {        
        /// <summary>
        /// Test Case 1: 
        /// Right = null
        /// Left = null
        /// Response Code: Not Found
        /// </summary>
        [TestMethod]
        public void Test1_Null_Right_Left_No_Result_Found()
        {
            V1Controller controller = new V1Controller();
            IHttpActionResult _result = controller.GetDiff(1);
            // Assert
            Assert.IsInstanceOfType(_result, typeof(NotFoundResult));
        }

        /// <summary>
        /// Test Case 2: 
        /// Right = null
        /// Left = "AAAAAA=="
        /// Response Code(PutLeft): Created
        /// Response Code(GetDiff) : Not Found
        /// </summary>
        [TestMethod]
        public void Test2_Assign_Only_Left_GetDiff_Result_Not_Found()
        {
            V1Controller controller = new V1Controller();
            Request _request=new Request();
            _request.data = "AAAAAA==";

            IHttpActionResult _resultGetPut = controller.PutLeft(1, _request);
            HttpStatusCode _PutStatusCode = ((StatusCodeResult)_resultGetPut).StatusCode;
            IHttpActionResult _resultGet = controller.GetDiff(1);
            // Assert
            Assert.IsInstanceOfType(_resultGet, typeof(NotFoundResult));
            Assert.AreEqual(_PutStatusCode, HttpStatusCode.Created);
        }

       
        /// <summary>
        ///Test Case 3: 
        ///Right = "AAAAAA=="
        ///Left = "AAAAAA=="
        ///Response Code(PutRight): Created
        ///Response Code(PutLeft) : Created
        ///Response Code(GetDiff) : Ok
        ///Response Content(GetDiff) : "Equals"
        /// </summary>
        [TestMethod]
        public void Test3_Right_Equals_Left_Result_Found()
        {
            V1Controller controller = new V1Controller();
            Request _request = new Request();
            _request.data = "AAAAAA==";
            IHttpActionResult _resultGetPutLeft = controller.PutLeft(1, _request);
            IHttpActionResult _resultGetPutRight = controller.PutRight(1, _request);
            IHttpActionResult _resultGet = controller.GetDiff(1);
            HttpStatusCode _PutRightStatusCode = ((StatusCodeResult)_resultGetPutRight).StatusCode;
            HttpStatusCode _PutLeftStatusCode = ((StatusCodeResult)_resultGetPutLeft).StatusCode;
            var contentResult = _resultGet as OkNegotiatedContentResult<DiffResponseWithoutLocation>;
            
            // Assert response codes
            Assert.AreEqual(_PutRightStatusCode, HttpStatusCode.Created);
            Assert.AreEqual(_PutLeftStatusCode, HttpStatusCode.Created);

            //Assert content
            Assert.IsNotNull(contentResult);
            Assert.AreEqual("Equals", contentResult.Content.diffResultType);
        }

        
        /// <summary>
        /// Test Case 4: 
        /// Right = "AAAAAA=="
        /// Left = "ABAAQW=="
        /// Response Code(PutRight): Created
        /// Response Code(PutLeft) : Created
        /// Response Code(GetDiff) : Ok
        /// Response Content(GetDiff) :
        /// "diffResultType": "ContentDoNotMatch",
        /// "diffLocations": [
        /// {
        ///     "offSet": 1,
        ///     "length": 1
        ///  },
        ///  {
        ///     "offSet": 3,
        ///     "length": 1
        ///  }
        ///  ]
        /// </summary>
        [TestMethod]
        public void Test4_Content_Do_Not_Match_Result_Found()
        {
            V1Controller controller = new V1Controller();
            Request _requestRight = new Request();
            _requestRight.data = "AAAAAA==";
            Request _requestLeft = new Request();
            _requestLeft.data = "ABAAQW==";
            IHttpActionResult _resultGetPutLeft = controller.PutLeft(1, _requestLeft);
            IHttpActionResult _resultGetPutRight = controller.PutRight(1, _requestRight);
            IHttpActionResult _resultGet = controller.GetDiff(1);
            HttpStatusCode _PutRightStatusCode = ((StatusCodeResult)_resultGetPutRight).StatusCode;
            HttpStatusCode _PutLeftStatusCode = ((StatusCodeResult)_resultGetPutRight).StatusCode;
            var contentResult = _resultGet as OkNegotiatedContentResult<DiffResponse>;
            // Assert response codes
            Assert.AreEqual(_PutRightStatusCode, HttpStatusCode.Created);
            Assert.AreEqual(_PutLeftStatusCode, HttpStatusCode.Created);

            //Assert content
            Assert.IsNotNull(contentResult);
            Assert.AreEqual("ContentDoNotMatch", contentResult.Content.diffResultType);
            Assert.AreEqual(2, contentResult.Content.diffLocations.Count);

            //Assert Diff offset and length
            List<DiffLocation> locList = (List<DiffLocation>)contentResult.Content.diffLocations;            
            Assert.AreEqual(1, locList[0].offSet);
            Assert.AreEqual(1, locList[0].length);
            Assert.AreEqual(3, locList[1].offSet);
            Assert.AreEqual(1, locList[1].length);
        }
               
        /// <summary>
        /// Test Case 4: 
        /// Right = "AAA="
        /// Left = "AAAAAA=="
        /// Response Code(PutLeft): Created
        /// Response Code(PutRight) : Created
        /// Response Code(GetDiff) : Ok
        /// Response Content(GetDiff) : "SizeDoNotMatch" 
        /// </summary>
        [TestMethod]
        public void Test5_Length_Do_Not_Match_Result_Found()
        {
            V1Controller controller = new V1Controller();
            Request _requestRight = new Request();
            _requestRight.data = "AAA=";
            Request _requestLeft = new Request();
            _requestLeft.data = "AAAAAA==";
            IHttpActionResult _resultGetPutLeft = controller.PutLeft(1, _requestLeft);
            IHttpActionResult _resultGetPutRight = controller.PutRight(1, _requestRight);
            IHttpActionResult _resultGet = controller.GetDiff(1);
            HttpStatusCode _PutStatusCode = ((StatusCodeResult)_resultGetPutRight).StatusCode;
            var contentResult = _resultGet as OkNegotiatedContentResult<DiffResponseWithoutLocation>;
            // Assert
            Assert.AreEqual(_PutStatusCode, HttpStatusCode.Created);
            Assert.IsNotNull(contentResult);
            Assert.IsNotNull(contentResult.Content);
            Assert.AreEqual("SizeDoNotMatch", contentResult.Content.diffResultType);
        }
    }
}
