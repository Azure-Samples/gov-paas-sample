using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace TrafficCaseApp.Models
{
    public class TrafficCase
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [Display(Name = "Date of Infraction")]
        [DataType(DataType.Date)]
        [Required]
        public DateTime? Date { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Violation { get; set; }

        [Required]
        public string License { get; set; }

        [Required]
        public string Status { get; set; }
       
    }
}
