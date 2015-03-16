namespace Subtitle.Provider.Addic7ed
{
    using System.Collections.Generic;

    public class AddictedSearchPage
    {
        public AddictedSearchPage(IEnumerable<FoundLink> foundLinks)
        {
            FoundLinks = foundLinks;
        }

        public IEnumerable<FoundLink> FoundLinks { get; private set; }
    }
}