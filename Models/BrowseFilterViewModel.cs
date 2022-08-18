using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhaseOneRecords.Models
{
    public class BrowseFilterViewModel
    {
        public string SortValue { get; set; }
        public List<string> GenreValues { get; set; }

        public List<string> Genres { get; set; }
        public string Message { get; set; }

        public string SearchArtist { get; set; } = null;
        public string SearchAlbum { get; set; } = null;
        public string SearchTerm { get; set; }

        public BrowseFilterViewModel()
        {

        }
        public BrowseFilterViewModel(List<string> tags)
        {
            Genres = tags.Count > 0 ? tags.OrderBy(w => w).ToList() : tags;
        }
    }
}
