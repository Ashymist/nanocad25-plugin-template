using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace NanoCadPlugin
{

    public partial class PluginForm : Form
    {
        public PluginForm()
        {
            InitializeComponent();
            calculateButton.Click += delegate { Calculate(); };
            FillDefaults();

            Text = PluginInfo.WindowTitle;
        }

        private void FillDefaults()
        {
            _input1.Text = "2";
            _input2.Text = "3";
        }

        private void Calculate()
        {
            var errors = new List<string>();

            double param1 = Read(_input1, "Параметр 1", errors);
            double param2 = Read(_input2, "Параметр 2", errors);

            if (errors.Count > 0)
            {
                ShowError(errors[0]);
                return;
            }

            var input = new PluginInput
            {
                Param1 = param1,
                Param2 = param2
            };

            try
            {
                PluginResult result = PluginCalculator.Calculate(input);

                _results.Text = result.Report;

                _status.ForeColor = Color.DarkGreen;
                _status.Text = "Расчёт выполнен.";
            }
            catch (System.Exception ex)
            {

                ShowError("Ошибка расчёта: " + ex.Message);
            }
        }

        private void ShowError(string message)
        {
            _status.ForeColor = Color.Firebrick;
            _status.Text = message;
        }

        private static double Read(TextBox box, string name, List<string> errors)
        {
            string text = box.Text.Trim().Replace(',', '.');
            double value;
            if (!double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out value))
            {
                errors.Add("Проверьте поле «" + name + "»: нужно число.");
                return 0;
            }
            return value;
        }

        private static void Set(TextBox box, double value, string format)
        {
            box.Text = value.ToString(format, CultureInfo.InvariantCulture);
        }
    }
}
