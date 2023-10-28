using System.Numerics;

namespace _3D_visualization.Model.Environment;

public class AutoMapper2D
{
    private List<Vector2>? _currentSection;

    private float _leftBorder;
    private float _rightBorder;
    private float _topBorder;
    private float _bottomBorder;

    public void Start(List<Vector2> currentSection)
    {
        if (currentSection.Count() < 3)
        {
            throw new System.Exception("");
        }
        
        _currentSection = currentSection;

        InitializeBorderBox(currentSection);
    }

    private void InitializeBorderBox(List<Vector2> currentSection)
    {
        _leftBorder = currentSection[0].X;
        _rightBorder = currentSection[0].X;
        
        _topBorder = currentSection[0].Y;
        _bottomBorder = currentSection[0].Y;

        for (int i = 1; i < currentSection.Count; i++)
        {
            if (currentSection[i].X > _rightBorder)
            {
                _rightBorder = currentSection[i].X;
            }
            
            if (currentSection[i].X < _leftBorder)
            {
                _leftBorder = currentSection[i].X;
            }
            
            if (currentSection[i].Y > _topBorder)
            {
                _topBorder = currentSection[i].Y;
            }
            
            if (currentSection[i].Y < _bottomBorder)
            {
                _bottomBorder = currentSection[i].Y;
            }
        }
    }

    // TODO add custom exception
    public Vector2 Lerp(Vector2 points)
    {
        if (_currentSection == null)
        {
            throw new System.Exception("");
        }

        return new Vector2((points.X - _leftBorder) / (_rightBorder - _leftBorder), (points.Y - _bottomBorder) / (_topBorder - _bottomBorder));
    }
    
    public void Finish()
    {
        _currentSection = null;
    }
}