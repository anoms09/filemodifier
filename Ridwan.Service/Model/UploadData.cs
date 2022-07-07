using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ridwan.Service.Model
{
    public class UploadData
    {
        public string Country { get; set; }
        public string ASN { get; set; }
        public long TTL { get; set; }
        public string IP { get; set; }
        public string Domain { get; set; }
    }
}
