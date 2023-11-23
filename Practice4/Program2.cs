namespace ModSys;

using System;
using ScottPlot;

public static class Program2
{

    public static void Main(string[] args)
    {
        // PrintLn("TZ");
        // for (int i = 0; i < 20; i++) {
        //     Print($"{RndTZ():0.00} ");
        // }
        // PrintLn();

        // PrintLn("TS");
        // for (int i = 0; i < 20; i++) {
        //     Print($"{RndTS():0.00} ");
        // }
        // PrintLn();

        // var task71 = TaskInBuffer(10000);
        // GeneratePlot(task71, "TimesInBuf");

        // var task72 = TaskInBufferProb(10000);
        // GeneratePlot(task72, "ProbInBuf");
    }

    static void Print(object? obj) => Console.Write(obj);
    static void PrintLn(object? obj) => Console.WriteLine(obj);
    static void PrintLn() => Console.WriteLine();

    static Random random = new Random();

    // 6. входной поток заявок
    static double RndTZ() {
        double lambda = 1.0/3.0;

        double rand = random.NextDouble();
        return -Math.Log(1 - rand) / lambda;
    }

    // 6. обработка сервером
    static double RndTS() {
        double mu = 0.25;

        double rand = random.NextDouble();
        return -1 / mu * Math.Log(rand);
    }

    // времена прихода
    static double[] TZTimes(int size)
    {
        double[] result = new double[size];

        for (int i = 0; i < size; i++)
        {
            double prevTime = 0.0;
            if (i > 0) prevTime = result[i - 1];
            result[i] = RndTZ() + prevTime;
        }

        return result;
    }

    // время нахождения в буфере
    static double[] TaskInBufferTime(int size)
    {
        double[] result = new double[size];
        double[] taskTimes = TZTimes(size);
        int computingIdx = 0;
        double time = 0.0;

        while (computingIdx < size - 1)
        {
            time += RndTS();

            double nextTaskTime = taskTimes[computingIdx + 1];
            if (time > nextTaskTime)
            {
                result[computingIdx + 1] = time - nextTaskTime;
            }
            else
            {
                time = nextTaskTime;
            }

            computingIdx++;
        }

        return result;
    }

    // 7. время нахождения в буфере одной/двух и тд программ
    static KeyValuePair<int, double>[] TaskInBuffer(int size)
    {
        Dictionary<int, double> result = new Dictionary<int, double>();
        double[] taskTimes = TZTimes(size);
        int computingIdx = 0;
        double time = 0.0;

        while (computingIdx < size - 1)
        {
            var computeTime = RndTS();
            time += computeTime;

            var bufi = 1;
            double lastBufTime = taskTimes[computingIdx + bufi];
            while (time > lastBufTime && bufi < size - 1 - computingIdx)
            {
                bufi++;
                lastBufTime = taskTimes[computingIdx + bufi];
            }

            if (result.ContainsKey(bufi - 1))
                result[bufi - 1] = result[bufi - 1] + computeTime;
            else
                result[bufi - 1] = computeTime;

            double nextTaskTime = taskTimes[computingIdx + 1];
            if (time < nextTaskTime)
                time = nextTaskTime;

            computingIdx++;
        }

        return result.ToList().OrderBy(kv => kv.Key).ToArray();
    }

    // 7. вероятность нахождения в буфере одной/двух программ
    static KeyValuePair<int, double>[] TaskInBufferProb(int size)
    {
        var taskInBuf = TaskInBuffer(size);
        var totalTimeInBuf = 0.0;
        for (int i = 0; i < taskInBuf.Count(); i++)
        {
            totalTimeInBuf += taskInBuf[i].Value;
        }
        for (int i = 0; i < taskInBuf.Count(); i++)
        {
            taskInBuf[i] = new KeyValuePair<int, double>(
                taskInBuf[i].Key,
                taskInBuf[i].Value / totalTimeInBuf
                );
        }
        return taskInBuf;
    }

    private static void GeneratePlot(KeyValuePair<int, double>[] values, string fileName) {
        var plt = new Plot(600, 400);
        var xs = values.Select(kv => (double) kv.Key).ToArray();
        var ys = values.Select(kv => kv.Value).ToArray();
        plt.PlotScatter(xs, ys);
        plt.SaveFig($"Graphics\\{fileName}.png");
    }
}