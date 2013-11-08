using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DemoThree
{
    static class Extensions
    {
        public static IEnumerable<Match> AsEnumerable(this MatchCollection collection)
        {
            foreach (Match match in collection)
            {
                yield return match;
            }
        }

        public static IEnumerable<Group> AsEnumerable(this GroupCollection collection)
        {
            foreach (Group group in collection)
            {
                yield return group;
            }
        }
    }
}
