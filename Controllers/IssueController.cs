using System;
using Microsoft.AspNetCore.Mvc;
using Sojourner.Services;
using Sojourner.Models;
using System.Linq;
using IdentityServer4;
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
        [Authorize(IdentityServerConstants.LocalApi.PolicyName)]
        [HttpGet("me/{id:regex([[0-9a-fA-F]]{{24}})}")]

        public async Task<Issue> getIssueByIdUser(string id)
        {
            var user = User.Claims.Where(claim => claim.Type == "Name").FirstOrDefault().Value;
            var res = await _issueService.getIssueById(id);
            if (res == null || res.uemail != user)
            {
                NotFound();
            }
            return res;
        }
        [Authorize(IdentityServerConstants.LocalApi.PolicyName)]
        [HttpGet("IssueByUid")]
        public async Task<List<Issue>> getIssueListByUid()
        {
            var uemail = User.Claims.Where(claim => claim.Type == "Name").FirstOrDefault().Value;
            Console.WriteLine("100");
            Console.WriteLine("101");
            var res = await _issueService.getIssueListByUid(uemail);
            Console.WriteLine("102");
            if (res == null)
                NotFound();
            Console.WriteLine("103");
            return res;
        }

        [Authorize("worker")]
        [HttpGet("IssueByWid")]
        public async Task<List<Issue>> getIssueListByWid()
        {
            var wemail = User.Claims.Where(claim => claim.Type == "Name").FirstOrDefault().Value;
            var res = await _issueService.getIssueListByWid(wemail);
            if (res == null)
                NotFound();
            return res;
        }
        [Authorize(IdentityServerConstants.LocalApi.PolicyName)]
        [HttpGet("IssueList")]
        public async Task<IActionResult> getIssueList()
        {
            Console.WriteLine("200");
            var res = await _issueService.getIssueList();
            Console.WriteLine("220");
            if (res == null)
                NotFound();
            Console.WriteLine("250");
            // var ok = Ok(res);
            Console.WriteLine("260");
            return Ok();
        }

        [Authorize(IdentityServerConstants.LocalApi.PolicyName)]
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
            var res = await _issueService.replyToComplain(issue.id, issue.reply, issue.needRepair);
            if (res == null)
                return StatusCode(StatusCodes.Status400BadRequest, new { success = false, error = "Reply is failed" });
            else
                return StatusCode(StatusCodes.Status200OK);
        }

        [Authorize("adminApi")]
        [HttpGet("sendWorker")]
        public async Task<IActionResult> sendWorker(string id, string workEmail)
        {
            var tmp = await _issueService.getIssueById(id);
            if (tmp.wemail != null)
                return StatusCode(StatusCodes.Status400BadRequest, new { success = false, error = "Already send worker" });
            var res = await _issueService.sendWorker(id, workEmail);

            if (res == null)
                return StatusCode(StatusCodes.Status400BadRequest, new { success = false, error = "Send worker fail" });
            else
                return StatusCode(StatusCodes.Status200OK);
        }

        [Authorize("adminApi")]
        [HttpGet("unFinishedIssue")]
        public async Task<List<Issue>> getUnFinishedIssueList()
        {
            var res = await _issueService.getUnFinishedIssueList();
            if (res == null)
                NotFound();
            return res;
        }

        // [Authorize("workerApi")]
        [HttpGet("needRepairIssue")]
        public async Task<List<Issue>> getNeedRepairIssueList()
        {
            var wemail = User.Claims.Where(claim => claim.Type == "Name").FirstOrDefault().Value;
            Console.WriteLine("2");
            var res = await _issueService.getNeedRepairIssueList(wemail);
            Console.WriteLine("3");
            if (res == null)
                NotFound();
            Console.WriteLine("4");
            return res;
        }

        // [Authorize("workerApi")]
        [HttpGet("confirmFinish")]
        public async Task<IActionResult> confirmFinish(String id)
        {
            var res = await _issueService.confirmFinish(id);
            return StatusCode(StatusCodes.Status200OK);
        }

        [Authorize(IdentityServerConstants.LocalApi.PolicyName)]
        [HttpDelete("{id:regex([[0-9a-fA-F]]{{24}})}")]
        public async Task<IActionResult> deleteIssue(string id)
        {
            var tem = await _issueService.deleteIssue(id);
            if (tem.DeletedCount != 1)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { success = false, error = "delete issue error" });
            }
            else
            {
                return StatusCode(StatusCodes.Status200OK);
            }
        }


    }
}
