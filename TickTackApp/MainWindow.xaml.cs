using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.InteropServices;

namespace TickTackApp
{
    public enum State
    {
        NotStarted,
        Running,
        Paused,
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private State isRunning = State.NotStarted;
        private Storyboard storyboard;

        public MainWindow()
        {
            InitializeComponent();

            storyboard = (Storyboard)FindResource("arcStoryboard");

            var duration = TimeSpan.FromMinutes(25);

            foreach (var anim in storyboard.Children)
            {
                anim.Duration = duration;
                if (anim.Name == "boolAnim")
                {
                    ((BooleanAnimationUsingKeyFrames)anim).KeyFrames[1].KeyTime = TimeSpan.FromSeconds(duration.TotalSeconds / 2); 
                }
            }
            Controls.Visibility = System.Windows.Visibility.Hidden;            
        }

        private void Path_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Rectangle_MouseEnter_1(object sender, MouseEventArgs e)
        {
            ((Rectangle)sender).Opacity = 1;
        }

        private void Path_MouseEnter_1(object sender, MouseEventArgs e)
        {
            Controls.Visibility = System.Windows.Visibility.Visible;
        }

        private void Path_MouseLeave_1(object sender, MouseEventArgs e)
        {
            Controls.Visibility = System.Windows.Visibility.Hidden;
        }

        private void Continue_MouseUp(object sender, MouseButtonEventArgs e)
        {
            storyboard.Duration = TimeSpan.FromMinutes(2);

            switch (isRunning)
            {
                case State.NotStarted:
                    storyboard.Begin();
                    pie.Visibility = System.Windows.Visibility.Visible;
                    isRunning = State.Running;
                    break;
                case State.Running:
                    storyboard.Pause();
                    isRunning = State.Paused;
                    break;
                case State.Paused:
                    storyboard.Resume();
                    isRunning = State.Running;
                    break;
                default:
                    break;
            }
        }

        private void Reset_MouseUp(object sender, MouseButtonEventArgs e)
        {
            storyboard.Begin();
            pie.Visibility = System.Windows.Visibility.Visible;
            isRunning = State.Running;

        }

        private void Canvas_MouseWheel_1(object sender, MouseWheelEventArgs e)
        {
            var storyboard = (Storyboard)FindResource("zoom");
            foreach(DoubleAnimation anim in storyboard.Children) {
                anim.From = anim.To;
                double newscale = (anim.From + ((e.Delta > 0) ? 0.10 : -0.10)).Value;

                anim.To = Math.Min(10, Math.Max(0.25, newscale));
            }
            storyboard.Begin();
        }
    }
}
