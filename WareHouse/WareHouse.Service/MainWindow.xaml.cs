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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.Entity;

namespace WareHouse.Service
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		WareHouseEntities context = new WareHouseEntities();
		CollectionViewSource userViewSource;

		public MainWindow()
		{
			InitializeComponent();
			userViewSource = ((CollectionViewSource)(FindResource("userViewSource")));
			DataContext = this;
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			//System.Windows.Data.CollectionViewSource userViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("userViewSource")));
			// Load data by setting the CollectionViewSource.Source property:
			// userViewSource.Source = [generic data source]
			context.Users.Load();
			userViewSource.Source = context.Users.Local;
		}
	}
}
