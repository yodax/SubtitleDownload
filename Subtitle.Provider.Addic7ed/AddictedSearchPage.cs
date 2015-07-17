using System.Collections.Generic;

namespace Subtitle.Provider.Addic7ed
{
    public class AddictedSearchPage
    {
        public AddictedSearchPage(IEnumerable<FoundLink> foundLinks)
        {
            FoundLinks = foundLinks;
        }

        public IEnumerable<FoundLink> FoundLinks { get; private set; }
    }
}