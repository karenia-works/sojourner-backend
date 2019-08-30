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

        /// <summary>
        /// Get issue according to issue id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize("adminApi")]
        [HttpGet("{id:regex([[0-9a-fA-F]]{{24}})}")]
        public async Task<Issue> getIssueById(string id)
        {
            var res = await _issueService.getIssueById(id);
            if (res == null)
            {
                NotFound();
            }
            return res;
        }

        [Authorize()]
        [HttpGet("IssueByUid")]
        public async Task<List<Issue>> getIssueListByUid(string uid)
        {
            var res = await _issueService.getIssueListByUid(uid);
            if (res == null)
                NotFound();
            return res;
        }

        [Authorize("worker")]
        [HttpGet("IssueByWid")]
        public async Task<List<Issue>> getIssueListByWid(string wid)
        {
            var res = await _issueService.getIssueListByWid(wid);
            if (res == null)
                NotFound();
            return res;
        }

        [Authorize()]
        [HttpGet("IssueList")]
        public async Task<List<Issue>> getIssueList()
        {
            var res = await _issueService.getIssueList();
            if(res == null)
                NotFound();
            return res;
        }

        [HttpPost]
        public async Task<IActionResult> insertIssue([FromBody] Issue issue)
        {
            issue.id = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
            await _issueService.insertIssue(issue);
            return StatusCode(StatusCodes.Status201Created, new { id = issue.id });
        }

        [Authorize("adminApi")]
        [HttpPut("{id:regex([[0-9a-fA-F]]{{24}})}")]
        public async Task<IActionResult> replyToComplain(string id, [FromBody] Issue issue)
        {
            var res = await _issueService.replyToComplain(issue.id, issue.reply,issue.needRepair);
            if (res == null)
                return StatusCode(StatusCodes.Status400BadRequest, new { success = false, error = "Reply is failed" });
            else
                return StatusCode(StatusCodes.Status200OK);
        }

        [Authorize("adminApi")]
        [HttpGet("sendWorker")]
        public async Task<IActionResult> sendWorker(string id, string workerId)
        {
            var tmp = await _issueService.getIssueById(id);
            if(tmp.wid != null)
                return StatusCode(StatusCodes.Status400BadRequest, new { success = false, error = "Already send worker" });
            var res = await _issueService.sendWorker(id, workerId);

            if (res == null)
                return StatusCode(StatusCodes.Status400BadRequest, new { success = false, error = "Send worker fail" });
            else
                return StatusCode(StatusCodes.Status200OK);
        }

        [Authorize("adminApi")]
        [HttpGet("unFinishedIssue")]
        public async Task<List<Issue>> getUnFinishedIssueList(string wid)
        {
            var res = await _issueService.getUnFinishedIssueList();
            if (res == null)
                NotFound();
            return res;
        }

        [Authorize("worker")]
        [HttpGet("needRepairIssue")]
        public async Task<List<Issue>> getNeedRepairIssueList(string wid)
        {
            var res = await _issueService.getNeedRepairIssueList(wid);
            if(res == null)
                NotFound();
            return res;
        }

        [Authorize("worker")]
        [HttpGet("confirmFinish")]
        public async Task<IActionResult> confirmFinish(string wid)
        {
            var res = await _issueService.confirmFinish(wid);
            return StatusCode(StatusCodes.Status200OK);
        }


    }
}
