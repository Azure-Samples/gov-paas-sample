using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrafficCaseApp.Models;
using TrafficCaseApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.Extensions.Caching.Distributed;
using System.Net;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace TrafficCaseApp.Controllers
{
    [Authorize]
    public class CasesController : Controller
    {
        private ITrafficCaseRepository caseRepository;
        private IQueueClient queueClient;

        public CasesController(ITrafficCaseRepository caseRepository, IQueueClient queueClient)
        {
            this.caseRepository = caseRepository;
            this.queueClient = queueClient;
        }

        // GET: Cases
        public IActionResult Index()
        {
            var cases = this.caseRepository.GetCases();
            return View(cases);
        }

        // GET: Cases/Create
        public async Task<IActionResult> Create()
        {
            CaseViewModel caseVM = new CaseViewModel();
            TrafficCase trafficCase = new TrafficCase();
            caseVM.Case = trafficCase;
            caseVM.Statuses = (await this.caseRepository.GetStatuses()).ToSelectList();
            return View(caseVM);
        }

        // POST: Cases/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind(Prefix = "Case")]TrafficCase trafficCase)
        {
            if (ModelState.IsValid)
            {
                trafficCase.Id = Guid.NewGuid();
                await this.caseRepository.CreateCase(trafficCase);
                await this.queueClient.AddCaseToQueue(trafficCase);
                return RedirectToAction(nameof(Index));
            }
            CaseViewModel caseVM = new CaseViewModel();
            caseVM.Case = trafficCase;
            caseVM.Statuses = (await this.caseRepository.GetStatuses()).ToSelectList();
            return View(caseVM);
        }

        // GET: Cases/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trafficCase = await this.caseRepository.GetCase(id);
            if (trafficCase == null)
            {
                return NotFound();
            }
            CaseViewModel caseVM = new CaseViewModel();
            caseVM.Case = trafficCase;
            caseVM.Statuses = (await this.caseRepository.GetStatuses()).ToSelectList();
            return View(caseVM);
        }

        // POST: Cases/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind(Prefix = "Case")]TrafficCase trafficCase)
        {
            CaseViewModel caseVM = new CaseViewModel();
            if (this.ModelState.IsValid)
            {
                caseVM.Case = trafficCase;
                caseVM.Statuses = (await this.caseRepository.GetStatuses()).ToSelectList();
                return View(caseVM);
            }
            else
            {
                await this.caseRepository.EditCase(trafficCase);
                await this.queueClient.AddCaseToQueue(trafficCase);
                return this.RedirectToAction("Index");
            }
        }

        //// GET: Cases/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var caseDelete = await this.caseRepository.GetCase(id);

            if (caseDelete == null)
            {
                return NotFound();
            }

            return View(caseDelete);
        }

        //// POST: Cases/Delete/5z
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await this.caseRepository.DeleteCase(id);
            return RedirectToAction(nameof(Index));
        }

        //Displays closed cases
        [Route("Cases/Closed")]
        public async Task<IActionResult> Closed()
        {
            this.ViewBag.Active = false;
            var list = await this.queueClient.GetClosedCases();
            return this.View("Closed", list);
        }
    }
}
