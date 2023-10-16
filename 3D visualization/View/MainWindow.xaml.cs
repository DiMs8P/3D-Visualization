using System.Diagnostics;
using System.Numerics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using _3D_visualization.Exception;
using _3D_visualization.ViewModel;
using Microsoft.Win32;
using SharpGL;
using SharpGL.WPF;
using Vector = System.Windows.Vector;

namespace _3D_visualization;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private ApplicationViewModel _applicationViewModel;
    public MainWindow()
    {
        InitializeComponent();
        
        _applicationViewModel = new ApplicationViewModel(OpenGLControl, 60);
        DataContext = _applicationViewModel;
    }
    
    private void OpenGLControl_OpenGLDraw(object sender, OpenGLRoutedEventArgs openGlRoutedEventArgs)
    {
        _applicationViewModel.Update(sender, openGlRoutedEventArgs);
    }
    
    private void OpenGLControl_OpenGLInitialized(object sender, OpenGLRoutedEventArgs openGlRoutedEventArgs)
    {
        _applicationViewModel.Initialize(sender, openGlRoutedEventArgs);
    } 

    private void OpenGLControl_Resized(object sender, OpenGLRoutedEventArgs openGlRoutedEventArgs)
    {
        _applicationViewModel.Resized(Width, Height);
    }

    private void Button_OpenFile(object sender, RoutedEventArgs e)
    {
        OpenFileDialog openFileDialog = new OpenFileDialog
        {
            DefaultExt = ".txt",
            Filter = "Text Document (.txt)|*.txt"
        };
        if (openFileDialog.ShowDialog() == true)
        {
            string fileName = openFileDialog.FileName;
            
            try
            {
                _applicationViewModel.SetReplicationObjects(fileName);
            }
            catch (ReplicationArgumentException exception)
            {
                ErrorTextBlock.Text = exception.What();
            }
        }
    }


    private void WireframeCheckbox_Unchecked(object sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void WireframeCheckbox_Checked(object sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void NormalCheckbox_Unchecked(object sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void NormalCheckbox_Checked(object sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void TextureCheckbox_Unchecked(object sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void TextureCheckbox_Checked(object sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void NormalSmoothingCheckbox_Unchecked(object sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void NormalSmoothingCheckbox_Checked(object sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void CameraComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        /*throw new NotImplementedException();*/
    }

    private void OpenGLControl_MouseMove(object sender, MouseEventArgs e)
    {
        _applicationViewModel.MouseHover(e.GetPosition(OpenGLControl));
    }

    private void OpenGLControl_KeyDown(object sender, KeyEventArgs e)
    {
        _applicationViewModel.KeyDown(sender, e);
    }
    
    private void OpenGLControl_KeyUp(object sender, KeyEventArgs e)
    {
        _applicationViewModel.KeyUp(sender, e);
    }
}