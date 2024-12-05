using System.Windows;

namespace WPF_Pokemon;

public partial class UserController : Window
{
    public UserController()
    {
        InitializeComponent();
    }
    
    private void OnClickGoToPageMainWindow(object sender, RoutedEventArgs args)
    {
        MainWindow mainController = new MainWindow();
        mainController.Show();
        this.Close();
    }
}