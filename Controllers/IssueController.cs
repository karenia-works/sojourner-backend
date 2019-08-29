using System;
using Microsoft.AspNetCore.Mvc;
using Sojourner.Services;
using Sojourner.Models;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Http;
using MongoDB;

namespace Sojourner.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class IssueController : ControllerBase
    {
        private IssueService _issueService;
        public IssueController(IssueService issuesService)
        {
            _issueService = issuesService;

        }

        [HttpGet("{id}")]
        public Issue getIssueById(string id)
        {
            var res = _issueService.getIssueById(id);
            if (res == null)
            {
                NotFound();
            }
            return res;
        }
        [HttpPost("insert")]
        public IActionResult insertIssue(Issue issue)
        {
            issue.id = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
            _issueService.insertIssue(issue);
            return StatusCode(StatusCodes.Status201Created, new { id = issue.id });
        }
        [HttpPut("update")]
        public IActionResult replyToComplain(Issue issue)
        {
            var res = _issueService.replyToComplain(issue.id,issue.reply);
            if(res == null)
                return StatusCode(StatusCodes.Status400BadRequest,new { success = false,error = "Reply is failed"});
            else
                return StatusCode(StatusCodes.Status200OK);
        }
        [HttpGet()]
        public IActionResult sendWorker(Issue issue)
        {
            var res = _issueService.sendWorker(issue.id,issue.wid);

            if(res == null)
                return StatusCode(StatusCodes.Status400BadRequest,new { success = false,error = "Send worker fail"});
            else
                return StatusCode(StatusCodes.Status200OK);
        }
        

    }
}
