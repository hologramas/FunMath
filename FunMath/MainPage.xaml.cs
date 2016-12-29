//
//
//

using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using ElementaryArithmetics;

namespace FunMath
{
    public sealed partial class MainPage : Page
    {
        private ArithmeticOperationGenerator operationGenerator;
        private StoryGenerator storyGenerator;
        private Story currentStory;
        private int currentStorySolution;
        private int correntAnswers;
        private int problemCounter;

        public MainPage()
        {
            this.InitializeComponent();

            var problemTypes = new ProblemType[] { ProblemType.FindTotal, ProblemType.FindLeftOperand, ProblemType.FindRightOperand };
            var operators = new Operator[] { Operator.Addition, Operator.Subtraction };

            this.operationGenerator = new ArithmeticOperationGenerator(10, operators);
            this.storyGenerator = new StoryGenerator(problemTypes);
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var storiesUri = new Uri("ms-appx:///Assets/Stories.json");
            var storyVarValuesUri = new Uri("ms-appx:///Assets/StoryElements.json");
            await this.storyGenerator.LoadStoryDataAsync(storiesUri, storyVarValuesUri);

            this.DisplayStoryProblem();
        }
        
        private void DisplayStoryProblem()
        {
            this.problemCounter++;

            this.DisplayTextBlock.Text = string.Empty;
            this.ResultTextBlock.Text = string.Empty;
            this.TotalTextBlock.Text = this.problemCounter.ToString();

            this.digitPad.Reset();

            var operation = this.operationGenerator.CreateOperation();

            this.currentStory = this.storyGenerator.CreateStory(operation);
            switch (this.currentStory.ProblemType)
            {
                case ProblemType.FindLeftOperand:
                    this.currentStorySolution = this.currentStory.Operation.LeftOperand.Resolve();
                    break;

                case ProblemType.FindRightOperand:
                    this.currentStorySolution = this.currentStory.Operation.RightOperand.Resolve();
                    break;

                case ProblemType.FindTotal:
                    this.currentStorySolution = this.currentStory.Operation.Resolve();
                    break;

                default:
                    throw new NotImplementedException();
            }

            this.DisplayTextBlock.Text += this.currentStory.StoryDescription;
        }

        private async void digitPad_OnTotalChanged(object sender, int e)
        {
            this.ResultTextBlock.Text = e.ToString();
            if (e == this.currentStorySolution)
            {
                this.correntAnswers++;
                await this.SwitchPanelsForAnsweredAsync(true);
                this.DisplayStoryProblem();
            }
            else if ((e > this.currentStorySolution) || ((e*10) > this.currentStorySolution))
            {
                await this.SwitchPanelsForAnsweredAsync(false);
                this.DisplayStoryProblem();
            }
        }

        private async Task SwitchPanelsForAnsweredAsync(bool isCorrectAnswer)
        {
            this.StarCountTextBlock.Text = this.correntAnswers.ToString();
            this.WorkPanel.Visibility = Visibility.Collapsed;

            if (isCorrectAnswer)
            {
                this.CorrectAnswerImagePanel.Visibility = Visibility.Visible;
            }
            else
            {
                this.IncorrectAnswerImagePanel.Visibility = Visibility.Visible;
            }

            await Task.Delay(2000);

            if (isCorrectAnswer)
            {
                this.CorrectAnswerImagePanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.IncorrectAnswerImagePanel.Visibility = Visibility.Collapsed;
            }

            this.WorkPanel.Visibility = Visibility.Visible;
        }
    }
}
