using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using population.Model;
using Serilog;

namespace population.Controllers
{
    [Route("api/v1/households")]
    public class HouseholdsController : Controller
    {
        private readonly PopulationContext _populationContext;
        private ILogger Logger { get; }

        public HouseholdsController()
        {
            _populationContext = PopulationContext.SingleInstance;
            Logger = Program.Logger;
        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public ActionResult Get([FromQuery]string state)
        {
            Logger.Information("API endpoint called - /households?state={0}", state);
            if (!string.IsNullOrEmpty(state))
            {
                string[] states = state.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (states.Length > 0)
                {
                    IList<HouseholdResult> list = new List<HouseholdResult>();
                    foreach (var s in states)
                    {
                        int sid;
                        try
                        {
                            sid = Convert.ToInt32(s);
                        }
                        catch (Exception e)
                        {
                            Logger.Error(e, "error convert string to int [" + s + "]");
                            continue;
                        }

                        if (!TryGetActuals(sid, list, false))
                        {
                            GetEstimates(sid, list);
                        }
                    }
                    if (list.Count > 0)
                    {
                        Logger.Information("API endpoint returned - 200");
                        return Ok(list);
                    }
                }
            }
            Logger.Error("API endpoint returned - 404");
            return NotFound();
        }

        private void GetEstimates(int sid, IList<HouseholdResult> list)
        {
            IDictionary<int, int> dlist = new Dictionary<int, int>();
            foreach (Estimate e in _populationContext.Estimates)
            {
                if (e.State == sid)
                {
                    if (dlist.ContainsKey(e.State))
                    {
                        dlist[e.State] = dlist[e.State] + e.EstimateHoseholds;
                    }
                    else
                    {
                        dlist[e.State] = e.EstimateHoseholds;
                    }
                }
            }

            if (dlist.Count > 0)
            {
                foreach (var kp in dlist)
                {
                    list.Add(new HouseholdResult { State = kp.Key, Households = kp.Value });
                }
            }
        }

        private bool TryGetActuals(int sid, IList<HouseholdResult> list, bool foundActuals)
        {
            foreach (var a in _populationContext.Actuals)
                if (a.State == sid)
                {
                    list.Add(new HouseholdResult
                    {
                        State = a.State,
                        Households = Math.Round(a.ActualHouseholds, 2)
                    });
                    foundActuals = true;
                }

            return foundActuals;
        }
    }

    public class HouseholdResult
    {
        public int State { get; set; }
        public double Households { get; set; }
    }
}
