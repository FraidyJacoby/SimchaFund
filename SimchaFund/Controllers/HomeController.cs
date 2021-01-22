using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SimchaFund.Data;
using SimchaFund.Models;

namespace SimchaFund.Controllers
{
    public class HomeController : Controller
    {
        private string _connectionString = @"Data Source=.\sqlexpress;Initial Catalog=SimchaFund;Integrated Security=True";

        public IActionResult Index()
        {
            var db = new SimchaFundDb(_connectionString);
            var vm = new IndexViewModel
            {
                Simchas = db.GetAllSimchas(),
                ContributorCount = db.GetContributorCount()
            };
            vm.Simchas = vm.Simchas.OrderBy(s => s.Date).ToList();
            return View(vm);
        }

        [HttpPost]
        public IActionResult AddSimcha(Simcha simcha)
        {
            var db = new SimchaFundDb(_connectionString);
            db.AddSimcha(simcha);
            return Redirect("/");
        }

        public IActionResult Contributions(int id)
        {
            var db = new SimchaFundDb(_connectionString);
            ContributionsViewModel vm = new ContributionsViewModel
            {
                Simcha = db.GetSimchaById(id),
                Contributors = db.GetContributors()
            };
            foreach (var contributor in vm.Contributors)
            {
                db.SetContributionAmountForContributorForSimcha(vm.Simcha.Id, contributor);
            }
            return View(vm);
        }

        [HttpPost]
        public IActionResult UpdateContributions(List<ContributionInclusion> contributors, int simchaId)
        {
            var db = new SimchaFundDb(_connectionString);
            db.UpdateContributions(contributors, simchaId);
            return Redirect("/");
        }

        public IActionResult Contributors()
        {
            var vm = new ContributorViewModel();
            var db = new SimchaFundDb(_connectionString);
            vm.Contributors = db.GetContributors();
            return View(vm);
        }

        [HttpPost]
        public IActionResult AddDeposit(Deposit deposit)
        {
            var db = new SimchaFundDb(_connectionString);
            db.AddDeposit(deposit);
            return Redirect("/home/contributors");
        }

        [HttpPost]
        public IActionResult AddContributorAndDeposit(Contributor contributor, int amount)
        {
            var db = new SimchaFundDb(_connectionString);
            int contributorId = db.AddContributor(contributor);
            var deposit = new Deposit
            {
                ContributorId = contributorId,
                Amount = amount,
                Date = DateTime.Today
            };
            db.AddDeposit(deposit);
            return Redirect("/home/contributors");
        }

        public IActionResult ShowHistory(int id)
        {
            var db = new SimchaFundDb(_connectionString);
            var vm = new ShowHistoryViewModel
            {
                Contributor = db.GetContributorById(id),
                Actions = db.GetActions(id)
            };

            vm.Actions = vm.Actions.OrderBy(a => a.Date).ToList<SimchaFund.Data.Action>();

            return View(vm);
        }

        public IActionResult EditContributor(Contributor contributor)
        {
            var db = new SimchaFundDb(_connectionString);
            db.EditContributor(contributor);
            return Redirect("/home/contributors");
        }
    }
}
