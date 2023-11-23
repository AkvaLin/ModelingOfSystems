using ScottPlot;

namespace ModSys;

public static class Program
{

    const int TZ_MIN = 4;
    const int TZ_MAX = 12;
    const int TS_MIN = 2;
    const int TS_MAX = 8;

    const int M = 1000;
    const int A_TZ = 39;
    const int A_TS = 39;
    const int B = 1;
    const int X0 = 1;

    public static void MainA(string[] args)
    {
        // PrintLn("TZ");
        // for (int i = 0; i < 20; i++) {
        //     Print(RndTZ(i) + " ");
        // }
        // PrintLn();

        // PrintLn("TS");
        // for (int i = 0; i < 20; i++) {
        //     Print(RndTS(i) + " ");
        // }
        // PrintLn();

        // var task2 = TZTimes(10);
        // for (int i = 0; i < 10; i++) {
        //     Print(task2[i] + " ");
        // }

        // var task3 = TaskInBufferTime(1000);
        // var cnt = 0;
        // for (int i = 0; i < 1000; i++) {
        //     if (task3[i] > 0.0)
        //         cnt++;
        // }
        // Print(cnt);

        // PrintLn($"Last task {TZTimes(1000)[999]}");

        // var task4 = TaskInBuffer(100);
        // foreach (var kv in task4)
        // {
        //     PrintLn($"{kv.Key} - {kv.Value}");
        // }

        // var task5 = TaskInBufferProb(100000);
        // foreach (var kv in task5)
        // {
        //     PrintLn($"{kv.Key} - {kv.Value}");
        // }
        
        var task71 = TaskInBuffer(10000);
        GeneratePlot(task71, "TimesInBufA");

        var task72 = TaskInBufferProb(10000);
        GeneratePlot(task72, "ProbInBufA");
    }

    static void Print(object? obj) => Console.Write(obj);
    static void PrintLn(object? obj) => Console.WriteLine(obj);
    static void PrintLn() => Console.WriteLine();

    static int Rnd(int a, int b, int m, int i, int x0)
    {
        if (i == 0) return x0;
        else return (a * Rnd(a, b, m, i - 1, x0) + b) % m;
    }

    // 1. входной поток заявок
    static double RndTZ(int i) =>
        Rnd(A_TZ, B, M, i, X0) *
        (TZ_MAX - TZ_MIN) /
        ((double)M) +
        TZ_MIN;

    // 1. обработка сервером
    static double RndTS(int i) =>
        Rnd(A_TS, B, M, i, X0) *
        (TS_MAX - TS_MIN) /
        ((double)M) +
        TS_MIN;

    // 2. времена прихода
    static double[] TZTimes(int size)
    {
        double[] result = new double[size];

        for (int i = 0; i < size; i++)
        {
            double prevTime = 0.0;
            if (i > 0) prevTime = result[i - 1];
            result[i] = RndTZ(i) + prevTime;
        }

        return result;
    }

    // 3. время нахождения в буфере
    static double[] TaskInBufferTime(int size)
    {
        double[] result = new double[size];
        double[] taskTimes = TZTimes(size);
        int computingIdx = 0;
        double time = 0.0;

        while (computingIdx < size - 1)
        {
            time += RndTS(computingIdx);

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

    // 4. время нахождения в буфере одной/двух и тд программ
    static KeyValuePair<int, double>[] TaskInBuffer(int size)
    {
        Dictionary<int, double> result = new Dictionary<int, double>();
        double[] taskTimes = TZTimes(size);
        int computingIdx = 0;
        double time = 0.0;

        while (computingIdx < size - 1)
        {
            var computeTime = RndTS(computingIdx);
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

    // 5. вероятность нахождения в буфере одной/двух программ
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