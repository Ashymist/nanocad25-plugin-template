namespace NanoCadPlugin
{
    partial class PluginForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            paramsGroup = new System.Windows.Forms.GroupBox();
            paramsGrid = new System.Windows.Forms.TableLayoutPanel();
            lblInput1 = new System.Windows.Forms.Label();
            _input1 = new System.Windows.Forms.TextBox();
            lblInput2 = new System.Windows.Forms.Label();
            _input2 = new System.Windows.Forms.TextBox();
            buttonPanel = new System.Windows.Forms.FlowLayoutPanel();
            calculateButton = new System.Windows.Forms.Button();
            _status = new System.Windows.Forms.Label();
            resultsGroup = new System.Windows.Forms.GroupBox();
            _results = new System.Windows.Forms.TextBox();
            paramsGroup.SuspendLayout();
            paramsGrid.SuspendLayout();
            buttonPanel.SuspendLayout();
            resultsGroup.SuspendLayout();
            SuspendLayout();

            paramsGroup.Controls.Add(paramsGrid);
            paramsGroup.Dock = System.Windows.Forms.DockStyle.Top;
            paramsGroup.Location = new System.Drawing.Point(0, 0);
            paramsGroup.Name = "paramsGroup";
            paramsGroup.Padding = new System.Windows.Forms.Padding(8, 6, 8, 6);
            paramsGroup.Size = new System.Drawing.Size(760, 110);
            paramsGroup.TabIndex = 2;
            paramsGroup.TabStop = false;
            paramsGroup.Text = "Исходные данные";

            paramsGrid.ColumnCount = 2;
            paramsGrid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 230F));
            paramsGrid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            paramsGrid.Controls.Add(lblInput1, 0, 0);
            paramsGrid.Controls.Add(_input1, 1, 0);
            paramsGrid.Controls.Add(lblInput2, 0, 1);
            paramsGrid.Controls.Add(_input2, 1, 1);
            paramsGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            paramsGrid.Location = new System.Drawing.Point(8, 22);
            paramsGrid.Margin = new System.Windows.Forms.Padding(0);
            paramsGrid.Name = "paramsGrid";
            paramsGrid.Padding = new System.Windows.Forms.Padding(4, 2, 4, 2);
            paramsGrid.RowCount = 2;
            paramsGrid.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            paramsGrid.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            paramsGrid.Size = new System.Drawing.Size(744, 82);
            paramsGrid.TabIndex = 0;

            lblInput1.Dock = System.Windows.Forms.DockStyle.Fill;
            lblInput1.Location = new System.Drawing.Point(7, 2);
            lblInput1.Margin = new System.Windows.Forms.Padding(3, 0, 6, 0);
            lblInput1.Name = "lblInput1";
            lblInput1.Size = new System.Drawing.Size(221, 32);
            lblInput1.TabIndex = 0;
            lblInput1.Text = "Параметр 1:";
            lblInput1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            _input1.Dock = System.Windows.Forms.DockStyle.Fill;
            _input1.Location = new System.Drawing.Point(237, 6);
            _input1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 2);
            _input1.Name = "_input1";
            _input1.Size = new System.Drawing.Size(114, 23);
            _input1.TabIndex = 1;

            lblInput2.Dock = System.Windows.Forms.DockStyle.Fill;
            lblInput2.Location = new System.Drawing.Point(7, 34);
            lblInput2.Margin = new System.Windows.Forms.Padding(3, 0, 6, 0);
            lblInput2.Name = "lblInput2";
            lblInput2.Size = new System.Drawing.Size(221, 32);
            lblInput2.TabIndex = 2;
            lblInput2.Text = "Параметр 2:";
            lblInput2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            _input2.Dock = System.Windows.Forms.DockStyle.Fill;
            _input2.Location = new System.Drawing.Point(237, 38);
            _input2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 2);
            _input2.Name = "_input2";
            _input2.Size = new System.Drawing.Size(114, 23);
            _input2.TabIndex = 3;

            buttonPanel.Controls.Add(calculateButton);
            buttonPanel.Controls.Add(_status);
            buttonPanel.Dock = System.Windows.Forms.DockStyle.Top;
            buttonPanel.Location = new System.Drawing.Point(0, 110);
            buttonPanel.Name = "buttonPanel";
            buttonPanel.Padding = new System.Windows.Forms.Padding(16, 7, 8, 4);
            buttonPanel.Size = new System.Drawing.Size(760, 46);
            buttonPanel.TabIndex = 1;
            buttonPanel.WrapContents = false;

            calculateButton.Location = new System.Drawing.Point(19, 10);
            calculateButton.Name = "calculateButton";
            calculateButton.Size = new System.Drawing.Size(170, 30);
            calculateButton.TabIndex = 0;
            calculateButton.Text = "Выполнить расчёт";
            calculateButton.UseVisualStyleBackColor = true;

            _status.AutoSize = true;
            _status.ForeColor = System.Drawing.Color.DimGray;
            _status.Location = new System.Drawing.Point(204, 15);
            _status.Margin = new System.Windows.Forms.Padding(12, 8, 0, 0);
            _status.Name = "_status";
            _status.Size = new System.Drawing.Size(0, 15);
            _status.TabIndex = 1;

            resultsGroup.Controls.Add(_results);
            resultsGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            resultsGroup.Location = new System.Drawing.Point(0, 156);
            resultsGroup.Name = "resultsGroup";
            resultsGroup.Padding = new System.Windows.Forms.Padding(8, 6, 8, 6);
            resultsGroup.Size = new System.Drawing.Size(760, 404);
            resultsGroup.TabIndex = 0;
            resultsGroup.TabStop = false;
            resultsGroup.Text = "Результаты";

            _results.BackColor = System.Drawing.Color.White;
            _results.Dock = System.Windows.Forms.DockStyle.Fill;
            _results.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            _results.Location = new System.Drawing.Point(8, 22);
            _results.Multiline = true;
            _results.Name = "_results";
            _results.ReadOnly = true;
            _results.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            _results.Size = new System.Drawing.Size(744, 376);
            _results.TabIndex = 0;
            _results.TabStop = false;
            _results.WordWrap = false;

            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(760, 560);
            Controls.Add(resultsGroup);
            Controls.Add(buttonPanel);
            Controls.Add(paramsGroup);
            Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimumSize = new System.Drawing.Size(740, 520);
            Name = "PluginForm";
            ShowIcon = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Мой плагин: расчёт";
            paramsGroup.ResumeLayout(false);
            paramsGrid.ResumeLayout(false);
            paramsGrid.PerformLayout();
            buttonPanel.ResumeLayout(false);
            buttonPanel.PerformLayout();
            resultsGroup.ResumeLayout(false);
            resultsGroup.PerformLayout();
            ResumeLayout(false);
        }

        private System.Windows.Forms.GroupBox paramsGroup;
        private System.Windows.Forms.TableLayoutPanel paramsGrid;
        private System.Windows.Forms.Label lblInput1;
        private System.Windows.Forms.TextBox _input1;
        private System.Windows.Forms.Label lblInput2;
        private System.Windows.Forms.TextBox _input2;
        private System.Windows.Forms.FlowLayoutPanel buttonPanel;
        private System.Windows.Forms.Button calculateButton;
        private System.Windows.Forms.Label _status;
        private System.Windows.Forms.GroupBox resultsGroup;
        private System.Windows.Forms.TextBox _results;
    }
}
