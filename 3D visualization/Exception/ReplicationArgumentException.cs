namespace _3D_visualization.Exception;

public class ReplicationArgumentException : System.Exception
{
    public int ErrorLine { get; }
    public string ErrorMessage { get; }

    public ReplicationArgumentException(int errorLine, string errorMessage)
    {
        ErrorLine = errorLine;
        ErrorMessage = errorMessage;
    }

    public string What()
    {
        return ($"Error in {ErrorLine} with message {ErrorMessage}");
    }
}

