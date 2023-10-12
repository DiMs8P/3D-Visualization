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
using _3D_visualization.ViewModel;
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
        float speed = 0.05f;

        if (Keyboard.IsKeyDown(Key.W))
        {
            cameraPos += speed * cameraFront;
        }
        if (Keyboard.IsKeyDown(Key.S))
        {
            cameraPos -= speed * cameraFront;
        }
        if (Keyboard.IsKeyDown(Key.A))
        {
            cameraPos -= Vector3.Normalize(Vector3.Cross(cameraFront, cameraUp)) * speed;
        }
        if (Keyboard.IsKeyDown(Key.D))
        {
            cameraPos += Vector3.Normalize(Vector3.Cross(cameraFront, cameraUp)) * speed;
        }
    }
    
    private void OpenGLControl_OpenGLInitialized(object sender, OpenGLRoutedEventArgs openGlRoutedEventArgs)
    {
        _applicationViewModel.OpenGLControl_OpenGLInitialized(sender, openGlRoutedEventArgs);
    } 

    private void OpenGLControl_Resized(object sender, OpenGLRoutedEventArgs openGlRoutedEventArgs)
    {
        _applicationViewModel.OpenGLControl_OpenGLResized(Width, Height);
    }

    private void Button_OpenFile(object sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void Button_DrawShape(object sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
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
        _applicationViewModel.OpenGLControl_MouseHover(sender, e);
    }

    private void OpenGLControl_KeyDown(object sender, KeyEventArgs e)
    {
        _applicationViewModel.OpenGLControl_KeyDown(sender, e);
    }
    
    private void OpenGLControl_KeyUp(object sender, KeyEventArgs e)
    {
        _applicationViewModel.OpenGLControl_KeyUp(sender, e);
    }
}