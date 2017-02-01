using System;
using System.Windows.Forms;
using Examples;
using Examples.Helpers;

namespace WindowsFormsExample
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox.Items.Clear();
            comboBox.Items.AddRange(ScenarioRunner.FindScenarios(typeof(Scenarios)));
            comboBox.SelectedIndex = 0;
        }

        private async void runButton_Click(object sender, EventArgs e)
        {
            var runner = comboBox.SelectedItem as ScenarioRunner;
            await runner?.RunScenarioAsync();
        }
    }
}
