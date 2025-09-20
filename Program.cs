using System;
using System.Net.NetworkInformation;
using System.Diagnostics;
using System.Threading;
using System.Collections.Generic;
using System.Media; // For system sounds

class Program
{
    static void Main(string[] args)
    {
        if (args.Length < 1)
        {
            Console.WriteLine("Usage: pingtool.exe <target> [-c count] [-s size] [-a success|failure]");
            Console.WriteLine("       If -c is not specified, ping will run continuously");
            Console.WriteLine("       -s size: Size of data to send in bytes (default: 56)");
            Console.WriteLine("       -a: Audio alert on success or failure (default: failure)");
            return;
        }

        string target = args[0];
        int count = int.MaxValue; // Default to continuous operation
        int dataSize = 56; // Default data size (like Linux ping)
        string alertMode = "failure"; // Default alert mode

        // Dictionary for option handlers
        var optionHandlers = new Dictionary<string, Action<string>>(StringComparer.OrdinalIgnoreCase)
        {
            ["-c"] = val => {
                if (int.TryParse(val, out int specifiedCount))
                    count = specifiedCount;
            },
            ["-s"] = val => {
                if (int.TryParse(val, out int size) && size > 0 && size <= 65500)
                    dataSize = size;
                else
                {
                    Console.WriteLine("Error: Size must be between 1 and 65500 bytes");
                    Environment.Exit(1);
                }
            },
            ["-a"] = val => {
                var mode = val.ToLower();
                if (mode == "success" || mode == "failure")
                    alertMode = mode;
                else
                {
                    Console.WriteLine("Error: Alert mode must be either 'success' or 'failure'");
                    Environment.Exit(1);
                }
            }
        };

        // Parse arguments using the dictionary
        for (int i = 1; i < args.Length; i++)
        {
            if (optionHandlers.TryGetValue(args[i], out var handler) && i + 1 < args.Length)
            {
                handler(args[i + 1]);
                i++; // Skip the value
            }
        }

        int timeout = 2000;
        List<long> roundTripTimes = new List<long>();
        int transmitted = 0;
        int received = 0;

        // Create buffer with specified size
        byte[] buffer = new byte[dataSize];
        for (int i = 0; i < buffer.Length; i++)
            buffer[i] = (byte)((i % 256) & 0xFF);

        Console.WriteLine($"PING {target} ({target}) {dataSize}({dataSize + 28}) bytes of data.");

        Ping ping = new Ping();
        bool isRunning = true;

        // Handle Ctrl+C to gracefully stop continuous ping
        Console.CancelKeyPress += (sender, e) => {
            e.Cancel = true;
            isRunning = false;
        };

        DateTime startTime = DateTime.Now;

        while (isRunning && transmitted < count)
        {
            transmitted++;
            try
            {
                Stopwatch sw = Stopwatch.StartNew();
                PingReply reply = ping.Send(target, timeout, buffer);
                sw.Stop();

                if (reply.Status == IPStatus.Success)
                {
                    received++;
                    roundTripTimes.Add(reply.RoundtripTime);
                    Console.WriteLine($"{dataSize} bytes from {reply.Address}: icmp_seq={transmitted} ttl={reply.Options?.Ttl ?? 0} time={reply.RoundtripTime} ms");
                    if (alertMode == "success")
                        SystemSounds.Asterisk.Play();
                }
                else
                {
                    Console.WriteLine($"From {target}: icmp_seq={transmitted} {reply.Status.ToString()}");
                    if (alertMode == "failure")
                        SystemSounds.Hand.Play();
                }

                Thread.Sleep(1000); // Standard 1 second interval like Linux ping
            }
            catch (PingException ex)
            {
                Console.WriteLine($"From {target}: icmp_seq={transmitted} Error: {ex.Message}");
                if (alertMode == "failure")
                    SystemSounds.Hand.Play();
            }
        }

        TimeSpan duration = DateTime.Now - startTime;

        // Print statistics when done or interrupted
        Console.WriteLine();
        Console.WriteLine($"--- {target} ping statistics ---");
        Console.WriteLine($"{transmitted} packets transmitted, {received} received, {(((transmitted - received) * 100.0) / transmitted):F1}% packet loss, time {(int)duration.TotalMilliseconds}ms");

        if (received > 0)
        {
            double avg = roundTripTimes.Average();
            double mdev = Math.Sqrt(roundTripTimes.Select(t => Math.Pow(t - avg, 2)).Average());
            Console.WriteLine($"rtt min/avg/max/mdev = {roundTripTimes.Min():F3}/{avg:F3}/{roundTripTimes.Max():F3}/{mdev:F3} ms");
        }
    }
}