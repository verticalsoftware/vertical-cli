﻿using System.Text;

namespace Vertical.Cli.Help;

public class HelpersTests
{
    [Fact]
    public Task Outputs_Expected()
    {
        var (p1, p2) = (
            "Everyone knows that paper is made from trees. But when one looks at trees, one cannot imagine that something so soft and fragile as the paper is made is so hard and strong. Plant materials such as wood are made of fibres known as cellulose. It is the primary ingredient in paper making. Raw wood is first converted into pulp consisting of a mixture of Cellulose, lignin, water and some chemicals. The pulp can be made mechanically through grinders or through chemical processes. Short fibres are produced by mechanical grinding. The paper produced in this way is weak and is used to make newspapers, magazines and phonebooks.",
            "Since March 8, 1990, Women’s Day has been observed by SAARC (South Asian Association for Regional Cooperation) comprising seven countries namely India, Pakistan, Nepal, Bhutan, Bangladesh, Sri Lanka and Maldives. The day is celebrated to highlight the problems of the girl child in these countries. It is very sad that girl children are subjected to extreme neglect and disrespect, especially in underdeveloped countries. The birth of a girl child is seen by parents as a cause of pity. They are deprived of proper nutrition, education, economic opportunities and social status or respect. We must eliminate these prejudices and provide a bright future for girls by educating them."
            );

        var str = $"{p1}\n\n{p2}";

        var sb = new StringBuilder();
        Helpers.BreakString(str, 80, (i, s) =>
        {
            if (i > 0) sb.AppendLine();
            sb.Append(s);
        });

        var result = sb.ToString();
        return Verify(result);
    }
}