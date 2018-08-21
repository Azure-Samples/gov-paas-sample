using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TrafficCaseApp.Models
{
    public class CaseViewModel
    {
        public TrafficCase Case { get; set; }
        public SelectList Statuses { get; set; }
    }
}
