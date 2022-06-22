using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneIdentityAnalyticsAPI.Models
{
    public class EmbeddedViewModel
    {
        public string tenantName { get; set; }
        public List<EmbeddedReport> reports { get; set; }
        public List<EmbeddedDataset> datasets { get; set; }
        public string embedToken { get; set; }
        public string embedTokenId { get; set; }
        public string user { get; set; }
        public bool userCanEdit { get; set; }
        public bool userCanCreate { get; set; }
    }
}
