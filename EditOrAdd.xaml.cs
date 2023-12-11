using System.Windows;

namespace Аудиоплеер;

public partial class EditOrAdd : Window
{
    public string LastName { get; set; } = "NewSong";
    public string? NameOfSong { get { return NewName.Text; } }

    public EditOrAdd()
    {
        InitializeComponent();
    }

    private void OkClick(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        NewName.Text = LastName;
    }

    private void UndoClick(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }
}