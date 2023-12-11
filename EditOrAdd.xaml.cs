using System.Windows;

namespace Аудиоплеер
{
    public partial class EditOrAdd : Window
    {
        public EditOrAdd()
        {
            InitializeComponent();
            
        }
        public string LastName { get; set; } = "NewSong";
        public string? NameOfSong { get {return NewName.Text;} }

        private void OkClick(object sender, RoutedEventArgs e)
        => this.DialogResult = true;
        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        => NewName.Text = LastName;
        
        private void UndoClick(object sender, RoutedEventArgs e)
        => this.DialogResult = false;
    }
}
