using EnvDTE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisGitCore.Services
{
    internal class Math
    {
        internal static int Percentage(int numerator, int denominator)
        {
            if (denominator == 0) return 0;
            return (int)System.Math.Round((double)(100 * numerator) / denominator);
        }

        internal static int RandomNumber(Random rnd, int min, int max)
        {
            return rnd.Next(min, max + 1); // max+1 because non-inclusive
        }
    }
}