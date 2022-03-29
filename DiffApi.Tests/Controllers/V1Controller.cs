using DiffApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;


namespace DiffApi.Controllers
{
    public class V1Controller : ApiController
    {
        static List<EndpointInput> _endPointList;
        
        //Constructor
        public  V1Controller()
        {
            if (_endPointList == null)
                _endPointList = new List<EndpointInput>();
        }
        // GET: api/V1/diff/1
        [HttpGet]
        [Route("api/V1/diff/{ID}")]
        public IHttpActionResult GetDiff(int ID)
        {
            EndpointInput _endPoint = _endPointList.Find(x => x.ID == ID);
            if (_endPoint == null)
            {
                return NotFound();
            }
            else if (string.IsNullOrEmpty(_endPoint.Left))
            {
                return NotFound();
            }
            else if (string.IsNullOrEmpty(_endPoint.Right))
            {
                return NotFound();
            }
            else if(_endPoint.Left == _endPoint.Right)
            {
                DiffResponseWithoutLocation diffResponse = new DiffResponseWithoutLocation();
                diffResponse.diffResultType = "Equals";
                return Ok(diffResponse);
            }
            else if(_endPoint.Left.Length != _endPoint.Right.Length)
            {
                DiffResponseWithoutLocation diffResponse = new DiffResponseWithoutLocation();
                diffResponse.diffResultType = "SizeDoNotMatch";
                return Ok(diffResponse);
            }
            else
            {
                DiffResponse diffResponse=new DiffResponse();
                diffResponse.diffResultType = "ContentDoNotMatch";
                CalculateDiffLocation(ref diffResponse, _endPoint);
                return Ok(diffResponse);
            }

        }
        //
        private void CalculateDiffLocation(ref DiffResponse diffResponse, EndpointInput _endPoint)
        {
            diffResponse.diffLocations=new List<DiffLocation>();
            DiffLocation _diffLocation;
            for (int i=0; i < _endPoint.Left.Length; i++)
            {
                if(_endPoint.Left[i]!= _endPoint.Right[i])
                {
                    _diffLocation=new DiffLocation();
                    _diffLocation.offSet = i;
                    _diffLocation.length = 1;
                    for (int j=i+1; j < _endPoint.Left.Length; j++)
                    {
                        if (_endPoint.Left[j] != _endPoint.Right[j])
                        {
                            _diffLocation.length++;
                        }
                        else
                        {
                            diffResponse.diffLocations.Add(_diffLocation);
                            i = j;
                            break;
                        }
                    }
                }
            }
            
        }


        // PUT: api/V1/diff/1/Left
        [HttpPut]
        [Route("api/V1/diff/{ID}/Left")]
        public IHttpActionResult PutLeft(int ID, [FromBody] Request Value)
        {
            if(string.IsNullOrEmpty(Value.data))
            {
                return BadRequest("Invalid input");
            }
            EndpointInput _endPoint = _endPointList.Find(x => x.ID == ID);
            if (_endPoint == null)
            {
                _endPoint=new EndpointInput();
                _endPoint.ID = ID;
                _endPoint.Left = Value.data;
                _endPointList.Add(_endPoint);
            }
            else
            {
                _endPoint.Left = Value.data;
            }
            return StatusCode(HttpStatusCode.Created);
        }

        // PUT: api/V1/diff/1/Right
        [HttpPut]
        [Route("api/V1/diff/{ID}/Right")]
        public IHttpActionResult PutRight(int ID, [FromBody] Request Value)
        {
            if (string.IsNullOrEmpty(Value.data))
            {
                return BadRequest("Invalid input");
            }
            EndpointInput _endPoint = _endPointList.Find(x => x.ID == ID);
            if (_endPoint == null)
            {
                _endPoint = new EndpointInput();
                _endPoint.ID = ID;
                _endPoint.Right = Value.data;
                _endPointList.Add(_endPoint);
            }
            else
            {
                _endPoint.Right = Value.data;
            }
            return StatusCode(HttpStatusCode.Created);
        }

    }
}
