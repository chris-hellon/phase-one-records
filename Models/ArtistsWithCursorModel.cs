using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhaseOneRecords.Models
{
    public class ArtistsWithCursorModel
    {
        public string Cursor { get; set; }
        public string Html { get; set; }
        public int PageFrom { get; set; } = 1;
    }
}
