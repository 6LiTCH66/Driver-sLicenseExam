using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Driver_sLicenseExam
{
    public partial class Form1 : Form
    {
        private Dictionary<string, List<string>> questionsMap;
        private FlowLayoutPanel mainFlowLayoutPanel;
        string[] correctAnswers = new string[] { "B", "D", "A", "A", "C", "A", "B", "A", "C", "D",
                                         "B", "C", "D", "A", "D", "C", "C", "B", "D", "A" };

        public Form1()
        {
            InitializeComponent();
            string fileName = "driver_license_questions.txt";
            string filePath = Path.Combine(Application.StartupPath, fileName);
            questionsMap = ParseQuestionsFileToHashmap(filePath);

            mainFlowLayoutPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false
            };

            this.Controls.Add(mainFlowLayoutPanel);
            DisplayQuestions();

            Button submitButton = new Button
            {
                Text = "Submit",
                AutoSize = true,
                Location = new System.Drawing.Point(10, 500) 
            };

            submitButton.Click += new EventHandler(SubmitButton_Click);
            mainFlowLayoutPanel.Controls.Add(submitButton);

        }

        private void SubmitButton_Click(object sender, EventArgs e)
        {
            CompareAnswers();
        }
        private Dictionary<string, List<string>> ParseQuestionsFileToHashmap(string filePath)
        {
            var questionsMap = new Dictionary<string, List<string>>();
            var fileLines = File.ReadAllLines(filePath);

            string currentQuestion = null;
            var currentAnswers = new List<string>();

            foreach (var line in fileLines)
            {
                if (line.Trim().EndsWith("?"))
                {
                    if (currentQuestion != null)
                    {
                        questionsMap.Add(currentQuestion, currentAnswers);
                    }

                    currentQuestion = line.Trim();
                    currentAnswers = new List<string>();
                }
                else if (line.Trim().StartsWith("A)") || line.Trim().StartsWith("B)") ||
                         line.Trim().StartsWith("C)") || line.Trim().StartsWith("D)"))
                {
                    currentAnswers.Add(line.Trim());
                }
            }

            if (currentQuestion != null)
            {
                questionsMap.Add(currentQuestion, currentAnswers);
            }

            return questionsMap;
        }
        private void DisplayQuestions()
        {
            int questionNumber = 1;

            foreach (var entry in questionsMap)
            {
                Label questionLabel = new Label
                {
                    Text = entry.Key,
                    AutoSize = true
                };
                mainFlowLayoutPanel.Controls.Add(questionLabel);

                GroupBox groupBox = new GroupBox
                {
                    AutoSize = true,
                    AutoSizeMode = AutoSizeMode.GrowAndShrink,
                    Padding = new Padding(3),
                    Text = String.Empty
                };
                mainFlowLayoutPanel.Controls.Add(groupBox);

                FlowLayoutPanel radioFlowLayoutPanel = new FlowLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    FlowDirection = FlowDirection.TopDown,
                    WrapContents = false,
                    AutoSize = true,
                    AutoSizeMode = AutoSizeMode.GrowAndShrink
                };
                groupBox.Controls.Add(radioFlowLayoutPanel);

                foreach (string answer in entry.Value)
                {
                    RadioButton radioButton = new RadioButton
                    {
                        Text = answer,
                        AutoSize = true,
                        Tag = $"Q{questionNumber}{answer.Substring(0, 1)}"
                    ,
                    };
                    radioFlowLayoutPanel.Controls.Add(radioButton);
                }

                mainFlowLayoutPanel.SetFlowBreak(groupBox, true);
                questionNumber++;
            }
        }

        private string[] GetUserAnswers()
        {
            string[] userAnswers = new string[questionsMap.Count];

            foreach (Control groupBox in mainFlowLayoutPanel.Controls)
            {
                if (groupBox is GroupBox)
                {
                    foreach (Control flowPanel in groupBox.Controls)
                    {
                        if (flowPanel is FlowLayoutPanel)
                        {
                            foreach (Control radioButton in flowPanel.Controls)
                            {
                                if (radioButton is RadioButton rb && rb.Checked)
                                {
                                    string tag = rb.Tag.ToString();
                                    int questionIndex = int.Parse(tag.Substring(1, tag.Length - 2)) - 1; // Get question index from tag
                                    userAnswers[questionIndex] = tag.Substring(tag.Length - 1); // Get the answer letter from the tag
                                }
                            }
                        }
                    }
                }
            }

            return userAnswers;
        }





        private void CompareAnswers()
        {
            string[] userAnswers = GetUserAnswers();
            int correctlyAnswered = 0;
            int incorrectlyAnswered = 0;
            List<int> incorrectlyAnsweredArray = new List<int>();
            string examResult = "";

            for (int i = 0; i < correctAnswers.Length; i++)
            {
                if (userAnswers[i] == correctAnswers[i])
                {
                    correctlyAnswered++;
                }
                else
                {
                    incorrectlyAnswered++;
                    incorrectlyAnsweredArray.Add(i + 1);

                }
            }

            if (correctlyAnswered >= 15)
            {
                examResult = "You have passed the exam";
            }
            else
            {
                examResult = "You have failed the exam";
            }


            MessageBox.Show($"{examResult}" +
                $"\nYou got {correctlyAnswered} out of {correctAnswers.Length} correct." +
                $"\nYou got {incorrectlyAnswered} out of {correctAnswers.Length} incorrectly" +
                $"\nQuestion numbers answered incorrectly: {String.Join(",", incorrectlyAnsweredArray.Select(p => p.ToString()).ToArray())}");
        }






    }
}
