using Config;
using Logging;
using System;
using System.Diagnostics;
using System.IO.Enumeration;
using System.Text;

namespace CLI
{
    class Program
    {
        const int ONE_HUNDRED = 100;
        const int ONE_THOUSAND = 1000;
        const int TEN_THOUSAND = 10000;
        const int ONE_HUNDRED_THOUSAND = 100000;
        const int ONE_MILLION = 1000000;
        const int TEN_MILLION = 10000000;

        static void Main(string[] args)
        {
            Log.Use(new ConsoleLogger(), LogLevels.Debug);

            var input_args = args.Parse<InputParameters>();

            string[] x = null;
            var y = x?.Length;
            Log.LogInfo($"y: {y == 0}");

            var sb = new StringBuilder();
            for(var i = 0; i < ONE_THOUSAND; i++)
            {
                sb.AppendLine($"[this_is_a_section_header_{i}]");
                for (var j = 0; j < ONE_HUNDRED; j++)
                {
                    sb.AppendLine($"this_is_a_key_{j}=this is a value {i} {j}");
                }
            }
            var s = sb.ToString();

            GC.Collect();
            GC.WaitForFullGCComplete();
            var startmem1 = GC.GetTotalAllocatedBytes(true);
            var sw = Stopwatch.StartNew();
            var config = new INIConfig(s);
            var endmem1 = GC.GetTotalAllocatedBytes(true);
            var elapsed1 = sw.ElapsedMilliseconds;
            var memdiff1 = endmem1 - startmem1;
            Log.LogInfo($"Elapsed: {elapsed1}");
            Log.LogInfo($"Mem Usage: {memdiff1}");
            Log.LogInfo($"First: {config["this_is_a_section_header_1", "this_is_a_key_1"]}");
            Log.LogInfo($"Last: {config["this_is_a_section_header_999", "this_is_a_key_99"]}");
            Log.LogInfo("");

            //Log.LogInfo($"{((elapsed1) / (float)elapsed2):P0} Faster");
            //Log.LogInfo($"{((memdiff1) / (float)memdiff2):P0} of the memory");
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    class InputKeyAttribute : Attribute
    {
        public InputKeyAttribute(string name)
        {

        }
    }

    class InputValidatorAttribute: Attribute
    {
        public InputValidatorAttribute(Type validator_type)
        {

        }
    }

    class InputUsageAttribute: Attribute
    {
        public InputUsageAttribute(string usage)
        {

        }
    }

    class InputRequiredAttribute: Attribute { }

    class FileExistsValidator
    {

    }

    static class ConsoleArguments
    {
        public static T Parse<T>(this string[] args)
        {
            if ((args?.Length ?? 0) == 0)
                return default;
            else
                return default;
        }
    }

    class InputParameters
    {
        [InputKey("f")]
        [InputKey("file")]
        [InputValidator(typeof(FileExistsValidator))]
        [InputRequired]
        [InputUsage("[-f, -file] c:\\path_to_file")]
        string FileName { get; set; }

        [InputKey("c")]
        bool Count { get; set; }
    }
}
