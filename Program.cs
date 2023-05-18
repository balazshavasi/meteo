using System;
using System.IO;
using System.Collections.Generic;

public class Data
{
    public List<Record> datas = new List<Record>();

    public double avg_temp_city(string _city)
    {
        double sum = 0;
        int counter = 0;
        foreach (var elem in datas)
        {
            if (elem.city == _city)
            {
                sum += elem.temp;
                counter++;
            }
        }

        double avg;
        if (counter != 0)
        {
            avg = sum / counter;
        }
        else
        {
            avg = 0;
        }

        return avg;
    }

    public double avg_temp_all()
    {
        double sum = 0;
        int counter = 0;
        foreach (var elem in datas)
        {
            sum += elem.temp;
            counter++;
        }
        if (counter != 0)
        {
            return sum / counter;
        }
        else
        {
            return 0;
        }
    }

    public Record min_temp()
    {
        double min_temp = double.MaxValue;
        Record min_record = new Record { city = "n.a.", temp = -1 };
        foreach (var elem in datas)
        {
            if (elem.temp < min_temp)
            {
                min_record = elem;
                min_temp = elem.temp;
            }
        }
        return min_record;
    }


    public Record max_temp()
    {
        double max_temp = double.MinValue;
        Record max_record = new Record { city = "n.a.", temp = -1 };
        foreach (var elem in datas)
        {
            if (elem.temp > max_temp)
            {
                max_record = elem;
                max_temp = elem.temp;
            }
        }
        return max_record;
    }

    public double avg_given_time_n_city(int _hour, string _city)
    {
        double sum = 0;
        int counter = 0;
        foreach (var elem in datas)
        {
            if (elem.city == _city)
            {
                if (elem.time.hour == _hour)
                {
                    sum += elem.temp;
                    counter++;
                }
            }
        }
        double avg;
        if (counter != 0)
        {
            avg = sum / counter;
        }
        else
        {
            avg = 0;
        }
        return avg;
    }

    public string max_avgtemp_city(int[] _hours, string _city)
    {
        List<double> avgs = new List<double>();
        foreach (var elem in _hours)
        {
            avgs.Add(avg_given_time_n_city(elem, _city));
        }
        double max = avgs.Max();
        int idx = avgs.IndexOf(max);

        return $"{_city}: at hour: {_hours.ElementAt(idx)}, max avg temp: {max.ToString("n1")}";
    }

    public Dictionary<double, int> classification(double range_length, double range_actual)
    {
        double maxtemp = max_temp().temp;
        Dictionary<double, int> rangelist = new Dictionary<double, int>();
        rangelist.Add(range_actual, 0);
        while (range_actual < maxtemp)
        {
            range_actual += range_length;
            rangelist.Add(range_actual, 0);
        }
        foreach (var elem in datas)
        {
            for (int i = 0; i < rangelist.Count(); i++)
            {
                if (elem.temp > rangelist.ElementAt(i).Key && elem.temp < rangelist.ElementAt(i + 1).Key)
                {
                    rangelist[rangelist.ElementAt(i).Key]++;             
                    break;
                }
            }
        }
        return rangelist;

    }

    public void dictionaryWriteOut(Dictionary<double, int> dict)
    {
        for (int i = 0; i < dict.Count() - 1; i++)
        {
            System.Console.WriteLine($"{dict.ElementAt(i).Key}-{dict.ElementAt(i + 1).Key}: {dict.ElementAt(i).Value} ");
        }
    }

    public List<double> avgs_days()
    {
        Dictionary<int, List<double>> dict = new Dictionary<int, List<double>>();
        foreach (var elem in datas)
        {
            if (!dict.ContainsKey(elem.day))
            {
                dict.Add(elem.day, new List<double>());
            }
            dict[elem.day].Add(elem.temp);
        }
        List<double> list = new List<double>();
        int idx = 0;
        foreach (var elem in dict)
        {
            list.Add(dict.ElementAt(idx).Value.Sum() / dict.ElementAt(idx).Value.Count());
            idx++;
        }
        return list;
    }


}

public struct Record
{
    public int day;
    public string city;

    public Time time;

    public double temp;

}

public struct Time
{
    public int hour;
    public int min;

    public override string ToString()
    {
        return $"{hour.ToString("00")}:{min.ToString("00")}";
    }
}



public class Program
{

    public static void Main()
    {

        Console.WriteLine("Program begin...");

        Data data = new Data();


        using (StreamReader sr = new StreamReader("hom_fl.txt"))
        {
            Record record;
            string? line;
            while ((line = sr.ReadLine()) != null)
            {
                string[] args = line.Split(' ');
                record.day = Int32.Parse(args[0]);
                record.city = args[1];
                string[] _time = args[2].Split(":");
                record.time.hour = int.Parse(_time[0]);
                record.time.min = int.Parse(_time[1]);
                //string replaced = args[3].Replace(",", ".");
                //record.temp = double.Parse(replaced);
                record.temp = double.Parse(args[3]);
                data.datas.Add(record);

                //System.Console.WriteLine($"first elem of args: {args.ElementAt(2)}, second: {args.ElementAt(3)}, datalength: {args.Length}" );
            }

            //1
            double avg_all = data.avg_temp_all();
            System.Console.WriteLine("avg_all: {0:n2}", avg_all);

            //2
            double avgdeb = data.avg_temp_city("DEB");
            System.Console.WriteLine("avg DEB: {0:n2}", avgdeb);

            double avgbp = data.avg_temp_city("BP");
            System.Console.WriteLine("avg BP: {0:n2}", avgbp);

            double avgsze = data.avg_temp_city("SZE");
            System.Console.WriteLine("avg SZE: {0:n2}", avgsze);

            //3
            Record _min_temp = data.min_temp();
            System.Console.WriteLine($"min temped day: {_min_temp.day}, city: {_min_temp.city}, temp: {_min_temp.temp.ToString("n1")}");

            //4
            Record _max_temp = data.max_temp();
            System.Console.WriteLine($"max temped day: {_max_temp.day}, city: {_max_temp.city}, temp: {_max_temp.temp.ToString("n1")}");

            //
            /*
            double _avg_2_BP =  data.avg_given_time_n_city(2, "BP");
            double _avg_14_BP =  data.avg_given_time_n_city(14, "BP");
            double _avg_22_BP =  data.avg_given_time_n_city(22, "BP");

            System.Console.WriteLine($"BP, avg at 2 hours: {_avg_2_BP}, at 14 hours: {_avg_14_BP}, at 22 hours: {_avg_22_BP}");
            */

            //5
            Dictionary<double, int> range_cntr = data.classification(10, 0);
            //System.Console.WriteLine(string.Join(Environment.NewLine, range_cntr) );
            data.dictionaryWriteOut(range_cntr);

            //6
            int[] hours = { 2, 14, 22 };
            System.Console.WriteLine($"{data.max_avgtemp_city(hours, "BP")}");

            //7
            List<double> list = data.avgs_days();
            int idx = 0;
            using (StreamWriter sw = new StreamWriter(new FileStream("meteo_kimenet_B4ZQ1W.txt", FileMode.Create)))
            {
                foreach (var elem in list)
                {
                    sw.WriteLine($"day: {++idx}, avg_day: {elem.ToString("n2")}");
                }
            }

        }

    }
}
