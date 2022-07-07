using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ridwan.Service.Model
{
    public class OutputModel
    {
        public int NoOfUniqueCountry { get; set; }
        public int NoOfUniqueASN { get; set; }
        public decimal AvgTTL { get; set; }
        public decimal VarianceTTL { get; set; }
        public int NoOfUniqueIP { get; set; }
        public int NoOfUniqueDomain { get; set; }

    }
}
