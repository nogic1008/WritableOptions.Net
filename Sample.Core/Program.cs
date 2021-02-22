using System;
using System.Linq;
using Sample.Core;

Console.WriteLine(
    string.Join(", ", Enumerable.Range(1, 20).Select(i => i.ToFizzBuzzFormat()))
);
