using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KoloDev.GDS.QA.Accelerator.Utility
{
    public static class QuerySelectorExtensions
    {
        public static HtmlNode NthOfTypeQuerySelector(this HtmlNode element, string selector)
        {
            if (selector.Contains("nth-of-type"))
            {
                var matches = Regex.Matches(selector, @"([a-zA-Z0-9.-]\s?)+(:nth-of-type\(\d\))");
                for (int i = 0; i < matches.Count; i++)
                {
                    var match = matches[i];
                    var splitty = match.Value.Split(':');
                    var elements = element.QuerySelectorAll(splitty[0]);
                    var n = int.Parse(Regex.Match(splitty[1], @"(\d)").Value);
                    var htmlNodes = elements.ToList();
                    element = (n - 1 < htmlNodes.Count) ? htmlNodes[n - 1] : htmlNodes[0];
                }
            }
            else
            {
                element = element.QuerySelector(selector);
            }
            return element;
        }
    }
}
