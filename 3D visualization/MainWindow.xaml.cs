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
    [DllImport("User32.dll")]
    private static extern bool SetCursorPos(int X, int Y);
    
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
        // TODO remove
        OpenGLControl.OpenGL.Enable(OpenGL.GL_DEPTH_TEST);
        OpenGLControl.OpenGL.Enable(OpenGL.GL_TEXTURE_2D);
        
        _applicationViewModel.Initialize(OpenGLControl, 60);
        _applicationViewModel.SetReplicationObjects("D:\\RiderProjects\\3D visualization\\3D visualization\\Source\\splineData.txt");
    } 

    private void OpenGLControl_Resized(object sender, OpenGLRoutedEventArgs openGlRoutedEventArgs)
    {
        OpenGL gl = OpenGLControl.OpenGL;

        gl.MatrixMode(OpenGL.GL_PROJECTION);

        //  Load the identity.
        gl.LoadIdentity();
        
        //  Create a perspective transformation.
        
        // Set Ortho projection
        //gl.Ortho(-8.0, 8.0, -8.0, 8.0, 0.01, 100.0);
        gl.Perspective(60.0f, OpenGLControl.ActualWidth / OpenGLControl.ActualHeight, 0.01, 100.0);

        //  Set the modelview matrix.
        gl.MatrixMode(OpenGL.GL_MODELVIEW);
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
        if (OpenGLControl.Focusable)
        {
            //Mouse.OverrideCursor = Cursors.None;
            
            _applicationViewModel.MouseHover(e.GetPosition(OpenGLControl)); 
            //SetCursor((int)App.Current.MainWindow.Width / 2, (int)App.Current.MainWindow.Height / 2);
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
    
    private static void SetCursor(int x, int y)
    {
        // Left boundary
        var xL = (int)App.Current.MainWindow.Left;
        // Right boundary
        var xR = xL + (int)App.Current.MainWindow.Width;
        // Top boundary
        var yT = (int)App.Current.MainWindow.Top;
        // Bottom boundary
        var yB = yT + (int)App.Current.MainWindow.Height;

        x += xL;
        y += yT;

        if (x < xL)
        {
            x = xL;
        }
        else if (x > xR)
        {
            x = xR;
        }

        if (y < yT)
        {
            y = yT;
        }
        else if (y > yB)
        {
            y = yB;
        }

        SetCursorPos(x, y);
    }
}