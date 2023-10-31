using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using _3D_visualization.Exception;
using _3D_visualization.ViewModel;
using Microsoft.Win32;
using SharpGL;
using SharpGL.Shaders;
using SharpGL.WPF;

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
        
        _applicationViewModel = new ApplicationViewModel();
        DataContext = _applicationViewModel;
    }
    
    private void OpenGLControl_OpenGLDraw(object sender, OpenGLRoutedEventArgs openGlRoutedEventArgs)
    {
    }
    
    private void OpenGLControl_OpenGLInitialized(object sender, OpenGLRoutedEventArgs openGlRoutedEventArgs)
    {
        OpenGLControl.OpenGL.Enable(OpenGL.GL_DEPTH_TEST);
        OpenGLControl.OpenGL.Enable(OpenGL.GL_TEXTURE_2D);
        OpenGLControl.OpenGL.Enable(OpenGL.GL_AUTO_NORMAL);
        
        _applicationViewModel.Initialize(OpenGLControl, 60);
        _applicationViewModel.SetReplicationObjects("D:\\RiderProjects\\3D visualization\\3D visualization\\Source\\splineData.txt");
    } 

    private void OpenGLControl_Resized(object sender, OpenGLRoutedEventArgs openGlRoutedEventArgs)
    {
        UpdateProjectionMatrix(((ComboBoxItem)CameraTypeComboBox.SelectedItem).Content.ToString());
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
        _applicationViewModel.UseWireframeMode(false);
    }

    private void WireframeCheckbox_Checked(object sender, RoutedEventArgs e)
    {
        _applicationViewModel.UseWireframeMode(true);
    }

    private void NormalCheckbox_Unchecked(object sender, RoutedEventArgs e)
    {
        _applicationViewModel.DrawNormals(false);
    }

    private void NormalCheckbox_Checked(object sender, RoutedEventArgs e)
    {
        _applicationViewModel.DrawNormals(true);
    }

    private void TextureCheckbox_Unchecked(object sender, RoutedEventArgs e)
    {
        _applicationViewModel.ShowTextures(false);
    }

    private void TextureCheckbox_Checked(object sender, RoutedEventArgs e)
    {
        _applicationViewModel.ShowTextures(true);
    }

    private void NormalSmoothingCheckbox_Unchecked(object sender, RoutedEventArgs e)
    {
        /*throw new NotImplementedException();*/
    }

    private void NormalSmoothingCheckbox_Checked(object sender, RoutedEventArgs e)
    {
        /*throw new NotImplementedException();*/
    }

    private void CameraComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (String.IsNullOrEmpty(CameraTypeComboBox.Text))
        {
            return;
        }
        
        UpdateProjectionMatrix(((ComboBoxItem)CameraTypeComboBox.SelectedItem).Content.ToString());
    }

    private void OpenGLControl_MouseMove(object sender, MouseEventArgs e)
    {
        if (OpenGLControl.Focusable)
        {
            _applicationViewModel.MouseHover(e.GetPosition(OpenGLControl)); 
        }
    }

    private void OpenGLControl_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            Mouse.OverrideCursor = null;
            OpenGLControl.Focusable = false;
        }
        
        if (OpenGLControl.Focusable)
        {
            _applicationViewModel.KeyDown(sender, e);
        }
    }
    
    private void OpenGLControl_KeyUp(object sender, KeyEventArgs e)
    {
        if (OpenGLControl.Focusable)
        {
            _applicationViewModel.KeyUp(sender, e);
        }
    }

    private void OpenGLControl_SetFocus(object sender, MouseButtonEventArgs e)
    {
        OpenGLControl.Focusable = true;
    }
    
    private void UpdateProjectionMatrix(string cameraType)
    {
        OpenGL gl = OpenGLControl.OpenGL;
        
        if (cameraType == "Ортографическая")
        {
            gl.MatrixMode(OpenGL.GL_PROJECTION);

            gl.LoadIdentity();
            gl.Ortho(-2, 2, -2, 2, 0.01, 100);
            
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
        }
        else if (cameraType == "Перспективная")
        {
            gl.MatrixMode(OpenGL.GL_PROJECTION);

            gl.LoadIdentity();
            gl.Perspective(60.0f, OpenGLControl.ActualWidth / OpenGLControl.ActualHeight, 0.01, 100.0);

            gl.MatrixMode(OpenGL.GL_MODELVIEW);
        }
    }
}