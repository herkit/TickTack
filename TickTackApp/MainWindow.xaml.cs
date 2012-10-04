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
using System.Diagnostics;

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
        private State timerState = State.NotStarted;
        private Phase pomodoroPhase = Phase.Pause;
        private Storyboard storyboard;
        private Storyboard flashing;

        public MainWindow()
        {
            InitializeComponent();
            storyboard = (Storyboard)FindResource("timerAnimationStoryBoard");
            flashing = (Storyboard)FindResource("flashingAnimation");

            SetDuration(TimeSpan.FromMinutes(1));

            storyboard.Completed += storyboard_Completed;

            Controls.Visibility = System.Windows.Visibility.Hidden;            
        }

        void storyboard_Completed(object sender, EventArgs e)
        {
            timerState = State.NotStarted;
        }

        private void SetDuration(TimeSpan duration)
        {
            storyboard.RepeatBehavior = new RepeatBehavior(duration);
            storyboard.Duration = duration;
            foreach (var anim in storyboard.Children)
            {
                Debug.WriteLine(anim.Name + " duration before: " + anim.Duration);
                anim.Duration = duration;
                Debug.WriteLine(anim.Name + " duration after: " + anim.Duration);
            }
        }

        private void Path_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
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

            switch (timerState)
            {
                case State.NotStarted:
                    StartPhase();
                    break;
                case State.Running:
                    storyboard.Pause();
                    flashing.Begin();
                    timerState = State.Paused;
                    break;
                case State.Paused:
                    storyboard.Resume();
                    flashing.Stop();
                    timerState = State.Running;
                    break;
                default:
                    break;
            }
        }

        private void StartPhase()
        {
            if (pomodoroPhase == Phase.Pause)
            {
                SetDuration(TimeSpan.FromMinutes(25));
                pomodoroPhase = Phase.Working;
            }
            else
            {
                SetDuration(TimeSpan.FromMinutes(5));
                pomodoroPhase = Phase.Pause;
            }

            var color = (Color)FindResource(pomodoroPhase.ToString());
            pie.Fill = new SolidColorBrush(color);

            storyboard.Begin();
            pie.Visibility = System.Windows.Visibility.Visible;
            timerState = State.Running;
        }
        private void Reset_MouseUp(object sender, MouseButtonEventArgs e)
        {
            storyboard.Begin();
            pie.Visibility = System.Windows.Visibility.Visible;
            timerState = State.Running;

        }

        private void Canvas_MouseWheel_1(object sender, MouseWheelEventArgs e)
        {
            var storyboard = (Storyboard)FindResource("zoom");
            foreach(DoubleAnimation anim in storyboard.Children) {
                anim.From = anim.To;
                double newscale = (anim.From + ((e.Delta > 0) ? 0.10 : -0.10)).Value;

                anim.To = Math.Min(1, Math.Max(0.25, newscale));
            }
            storyboard.Begin();
        }
    }
}
