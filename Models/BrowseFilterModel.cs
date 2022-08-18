using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhaseOneRecords.Models
{
    public class BrowseFilterModel
    {
        public string SortValue { get; set; }
        public List<string> GenreValues { get; set; }

        public BrowseFilterModel(string sortValue, string genreValues = null)
        {
            SortValue = sortValue;

            if (genreValues != null)
                GenreValues = genreValues.Split(",").ToList();
        }
        public string GetAppliedFilterLabel()
        {
            return SortingValues[SortValue];
        }

        public Dictionary<string, string> SortingValues { get; set; } = new Dictionary<string, string>()
                {
                    { "sortKey: VENDOR", "ARTIST // A-Z"  },
                    { "sortKey: VENDOR, reverse: true", "ARTIST // Z-A" },
                    { "sortKey: TITLE", "ALBUM // A-Z" },
                    { "sortKey: TITLE, reverse: true", "ALBUM // Z-A" },
                    { "sortKey: CREATED_AT", "NEWEST" },
                    { "sortKey: CREATED_AT, reverse: true", "OLDEST" },
                };
}
}
