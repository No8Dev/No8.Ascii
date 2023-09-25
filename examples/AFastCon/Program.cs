
using System.Diagnostics;

using var input = new StreamReader(
    Console.OpenStandardInput(),
    bufferSize: 16384);

using var output = new StreamWriter(
    Console.OpenStandardOutput(),
    bufferSize: 16384);

var width = Console.WindowWidth;
var height = Console.WindowHeight;

var sw = new Stopwatch();
sw.Start();
var count = 0;
var lastCount = 0;

while (true)
{
    output.Write($"\x1b[{0};{0}H");

    var max = width * height - 1;
    var rnd = Random.Shared.Next('a', 'z');
    for (int i = 0; i < max; i++)
        output.Write((char)rnd);
    count++;
    
    if (sw.ElapsedMilliseconds > 1000)
    {
        lastCount = count;
        count = 0;
        sw.Restart();
    }
    output.Write($"\x1b[{0};{0}H {lastCount} <<");
    output.Flush();

    if (Console.KeyAvailable)
    {
        var x = Console.ReadKey();
        if (x.KeyChar is 'x' or 'q')
            break;
    }
}
