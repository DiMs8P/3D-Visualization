using System.IO;
using System.Text;
using SharpGL;

namespace _3D_visualization.Model.SystemComponents.Render;

public class Shader
{
    private OpenGL _gl;
    private uint _id;
    
    public Shader(OpenGL gl, string vertexPath, string fragmentPath, string? geometryPath = null)
    {
        _gl = gl;
        
        string vertexCode;
        string fragmentCode;
        string geometryCode = "";
        
        vertexCode = File.ReadAllText(vertexPath);
        fragmentCode = File.ReadAllText(fragmentPath);
        if (geometryPath != null)
        {
            geometryCode = File.ReadAllText(geometryPath);
        }

        uint vertex, fragment, geometry = 0;

        vertex = gl.CreateShader(OpenGL.GL_VERTEX_SHADER);
        gl.ShaderSource(vertex, vertexCode);
        gl.CompileShader(vertex);
        CheckCompileErrors(gl, vertex, "VERTEX");
        
        fragment = gl.CreateShader(OpenGL.GL_FRAGMENT_SHADER);
        gl.ShaderSource(fragment, fragmentCode);
        gl.CompileShader(fragment);
        CheckCompileErrors(gl, fragment, "FRAGMENT");
        
        if (geometryPath != null)
        {
            geometry = gl.CreateShader(OpenGL.GL_GEOMETRY_SHADER);
            gl.ShaderSource(geometry, geometryCode);
            gl.CompileShader(geometry);
            CheckCompileErrors(gl, geometry, "GEOMETRY");
        }

        _id = gl.CreateProgram();
        gl.AttachShader(_id, vertex);
        gl.AttachShader(_id, fragment);
        if (geometryPath != null)
        {
            gl.AttachShader(_id, geometry);
        }
        
        gl.LinkProgram(_id);
        CheckCompileErrors(gl, _id, "PROGRAM");
        
        gl.DeleteShader(vertex);
        gl.DeleteShader(fragment);
        if (geometryPath != null)
        {
            gl.DeleteShader(geometry);
        }
    }

    public void Use()
    {
        _gl.UseProgram(_id);
    }
    
    public void SetBool(string name, bool value)
    {
        _gl.Uniform1(_gl.GetUniformLocation(_id, name), value == true ? 1 : 0);
    }
    // ------------------------------------------------------------------------
    public void SetInt(string name, int value)
    {
        _gl.Uniform1(_gl.GetUniformLocation(_id, name), value);
    }
    // ------------------------------------------------------------------------
    public void SetFloat(string name, float value)
    {
        _gl.Uniform1(_gl.GetUniformLocation(_id, name), value);
    }
    // ------------------------------------------------------------------------
    public void SetVec2(string name, float first, float second)
    {
        _gl.Uniform2(_gl.GetUniformLocation(_id, name), first, second);
    }
    // ------------------------------------------------------------------------
    public void SetVec3(string name, float first, float second, float third)
    {
        _gl.Uniform3(_gl.GetUniformLocation(_id, name), first, second, third);
    }
    // ------------------------------------------------------------------------
    public void SetVec4(string name, float first, float second, float third, float forth)
    {
        _gl.Uniform4(_gl.GetUniformLocation(_id, name), first, second, third, forth);
    }
    // ------------------------------------------------------------------------
    public void SetMat2(string name, float[] values)
    {
        _gl.UniformMatrix2(_gl.GetUniformLocation(_id, name), 1, false, values);
    }
    // ------------------------------------------------------------------------
    public void SetMat3(string name, float[] values)
    {
        _gl.UniformMatrix3(_gl.GetUniformLocation(_id, name), 1, false, values);
    }
    // ------------------------------------------------------------------------
    public void SetMat4(string name, float[] values)
    {
        _gl.UniformMatrix4(_gl.GetUniformLocation(_id, name), 1, false, values);
    }
    
    private void CheckCompileErrors(OpenGL gl, uint shader, string type)
    {
        int[] success = new int[1];
        StringBuilder stringBuilder = new StringBuilder();
        
        if (type != "PROGRAM")
        {
            gl.GetShader(shader, OpenGL.GL_COMPILE_STATUS, success);
            if (success[0] == 0)
            {
                gl.GetShaderInfoLog(shader, 1024, IntPtr.Zero, stringBuilder);
                Console.WriteLine($"ERROR::SHADER_COMPILATION_ERROR of type: {type} \n {stringBuilder.ToString()} \n -- --------------------------------------------------- --");
            }
        }
        else
        {
            gl.GetProgram(shader, OpenGL.GL_LINK_STATUS, success);
            if (success[0] == 0)
            {
                gl.GetProgramInfoLog(shader, 1024, IntPtr.Zero, stringBuilder);
                Console.WriteLine($"ERROR::PROGRAM_LINKING_ERROR of type: {type} \n {stringBuilder.ToString()} \n -- --------------------------------------------------- --");
            }
        }
    }
}