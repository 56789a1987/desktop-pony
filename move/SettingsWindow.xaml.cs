using System;
using System.Windows;

namespace move
{
	/// <summary>
	/// SettingsWindow.xaml 的交互逻辑
	/// </summary>
	public partial class SettingsWindow : Window
	{
		MainWindow main;
		private double xSpeed {
			set
			{
				main.xSpeed = value;
			}
			get
			{
				return main.xSpeed;
			}
		}
		private double flySpeed
		{
			set
			{
				main.upSpeed = value;
			}
			get
			{
				return main.upSpeed;
			}
		}
		private double gravity
		{
			set
			{
				main.g = value;
			}
			get
			{
				return main.g;
			}
		}
		private double mouseSpeed
		{
			set
			{
				main.mouseSpeed = value;
			}
			get
			{
				return main.mouseSpeed;
			}
		}
		private double lastXSpeed, lastFlySpeed, lastGravity, lastMouseSpeed;
		public SettingsWindow(MainWindow window)
		{
			main = window;
			InitializeComponent();
			updateLabelAndSliders();

			lastXSpeed = xSpeed;
			lastFlySpeed = flySpeed;
			lastGravity = gravity;
			lastMouseSpeed = mouseSpeed;
		}

		private void updateLabelAndSliders()
		{
			walkSlider.Value = xSpeed;
			flySlider.Value = flySpeed;
			gravitySlider.Value = gravity;
			mouseSlider.Value = mouseSpeed;

			walkLabel.Content = xSpeed;
			flyLabel.Content = flySpeed;
			gravityLabel.Content = String.Format("{0:N1}", gravity);
			mouseLabel.Content = String.Format("{0:N3}", mouseSpeed);
		}

		private void updateXSpeed(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			xSpeed = walkSlider.Value;
			walkLabel.Content = xSpeed;
		}

		private void updateFlySpeed(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			flySpeed = flySlider.Value;
			flyLabel.Content = flySpeed;
		}

		private void updateGravity(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			gravity = gravitySlider.Value;
			gravityLabel.Content = String.Format("{0:N1}", gravity);

		}

		private void updateMouseSpeed(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			mouseSpeed = mouseSlider.Value;
			mouseLabel.Content = String.Format("{0:N3}", mouseSpeed);
		}

		private void Reset_Click(object sender, RoutedEventArgs e)
		{
			xSpeed = 5;
			flySpeed = 10;
			gravity = 0.5;
			mouseSpeed = 0.015;
			updateLabelAndSliders();
		}

		private void Cancel_Click(object sender, RoutedEventArgs e)
		{
			xSpeed = lastXSpeed;
			flySpeed = lastFlySpeed;
			gravity = lastGravity;
			mouseSpeed = lastMouseSpeed;
			Close();
		}

		private void OK_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}
	}
}
