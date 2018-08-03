using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using population.Model;
using Serilog;

namespace population.Controllers
{
    [Route("/api/v1/population")]
    [EnableCors("AllowSpecificOrigin")]
    public class PopulationController : Controller
    {
        private readonly PopulationContext _populationContext;
        private ILogger Logger { get; }

        public PopulationController() {
            _populationContext = PopulationContext.SingleInstance;
            Logger = Program.Logger;
        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public ActionResult Get([FromQuery]string state) {

            Logger.Information("API endpoint called - /population?state={0}", state);
            if(!string.IsNullOrEmpty(state)) {
                var states = state.Split(new []{','});
                if(states.Length > 0) {
                    IList<PopulationResult> list = new List<PopulationResult>();
                    foreach(var s in states)
                    {
                        int sid;
                        try
                        {
                            sid = Convert.ToInt32(s);
                        }
                        catch (Exception e)
                        {
                            Logger.Error(e, "error converting State to number [" + s + "]");
                            continue;
                        }
                        if(!TryGetActuals(sid, list, false))
                        {
                            GetEstimates(sid, list);
                        }
                    }
                    if(list.Count>0) {
                        Logger.Information("API endpoint returned - 200");
                        return Ok(list);
                    }
                }
            }
            Logger.Error("API endpoint returned - 404");
            return NotFound();
        }

        private void GetEstimates(int sid, IList<PopulationResult> list)
        {
            IDictionary<int, int> dlist = new Dictionary<int, int>();
            foreach (var e in _populationContext.Estimates)
            {
                if (e.State == sid)
                {
                    if (dlist.ContainsKey(e.State))
                    {
                        dlist[e.State] = dlist[e.State] + e.EstimatesPopulation;
                    }
                    else
                    {
                        dlist[e.State] = e.EstimatesPopulation;
                    }
                }
            }

            if (dlist.Count > 0)
            {
                foreach (var kp in dlist)
                {
                    list.Add(new PopulationResult {State = kp.Key, Population = kp.Value});
                }
            }
        }

        private bool TryGetActuals(int sid, IList<PopulationResult> list, bool foundActuals)
        {
            foreach (var a in _populationContext.Actuals)
                if (a.State == sid)
                {
                    list.Add(new PopulationResult
                    {
                        State = a.State,
                        Population = Math.Round(a.ActualPopulation, 2)
                    });
                    foundActuals = true;
                }

            return foundActuals;
        }
    }
    
    public class PopulationResult {
        public int State {get;set;}
        public double Population {get;set;}
    }
}