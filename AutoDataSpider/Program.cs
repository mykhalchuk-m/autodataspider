using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using EfTidyNet;
using System.Text.RegularExpressions;

namespace AutoDataSpider
{
    class Program
    {
        static void Main(string[] args)
        {
            WebTransformator webTransformator = new WebTransformator();
            webTransformator.RunTransformation();
        }
    }
}
