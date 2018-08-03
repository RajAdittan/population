using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace population.Model {
    [Table("Actuals")]
    public class Actual {
        [Key]
        [Column(Order=1)]
        public int State {get;set;}
        [Column(Order=2)]
        public double ActualPopulation{get;set;}
        [Column(Order=3)]
        public double ActualHouseholds {get;set;}
    }
}