using System.Globalization;
using System.Text;

namespace NanoCadPlugin
{

    internal sealed class PluginInput
    {
        public double Param1;
        public double Param2;
    }

    internal sealed class PluginResult
    {
        public double Sum;
        public double Product;

        public string Report;
    }

    internal static class PluginCalculator
    {

        internal static PluginResult Calculate(PluginInput input)
        {
            var result = new PluginResult();

            result.Sum = input.Param1 + input.Param2;
            result.Product = input.Param1 * input.Param2;

            var report = new StringBuilder();
            report.AppendLine("Результаты расчёта");
            report.AppendLine("------------------");
            report.AppendLine("Параметр 1:   " + Fmt(input.Param1));
            report.AppendLine("Параметр 2:   " + Fmt(input.Param2));
            report.AppendLine();
            report.AppendLine("Сумма:        " + Fmt(result.Sum));
            report.AppendLine("Произведение: " + Fmt(result.Product));
            report.AppendLine();
            report.AppendLine("Это заглушка шаблона.");
            report.AppendLine("Замените тело PluginCalculator.Calculate() своим расчётом.");

            result.Report = report.ToString();
            return result;
        }

        internal static string Fmt(double value, string format = "0.####")
        {
            return value.ToString(format, CultureInfo.InvariantCulture);
        }
    }
}
