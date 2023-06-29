using UnityEditor;
using UnityEngine;

public class MiniGameGrid
{
    public float Zoom
    {
        get
        {
            return _zoom;
        }
        set
        {
            _zoom = value > MAX_ZOOM_VALUE ? MAX_ZOOM_VALUE : value < MIN_ZOOM_VALUE ? MIN_ZOOM_VALUE : value;
        }
    }
    public bool IsDrawCenter;

    private readonly EditorWindow _parentWindow;

    private Vector2 _offset;
    private float _zoom;


    #region public methods
    public Vector2 GetStartPoint()
    {
        return _offset;
    }
    public Vector2 RoundCoordsToInt(Vector2 vec)
    {
        return new Vector2(Mathf.RoundToInt(vec.x), Mathf.RoundToInt(vec.y));
    }

    public Vector2 GUIToGrid(Vector3 vec)
    {
        Vector2 newVec = (
            new Vector2(vec.x, -vec.y) - new Vector2(_parentWindow.position.width / 2, -_parentWindow.position.height / 2))
            * _zoom + new Vector2(_offset.x, -_offset.y);
        return RoundCoordsToInt(newVec);
    }
    public Vector2 GridToGUI(Vector3 vec)
    {
        return (new Vector2(vec.x - _offset.x, -vec.y - _offset.y)) / _zoom
            + new Vector2(_parentWindow.position.width / 2, _parentWindow.position.height / 2);
    }

    public Vector2 FindPoint(float angle, float radius)
    {
        return new Vector2(radius * Mathf.Cos((270 + angle) * Mathf.Deg2Rad), radius * Mathf.Sin((270 + angle) * Mathf.Deg2Rad));
    }

    public void Draw()
    {
        //DrawLines();
        DrawCenter();
    }

    public void Recenter()
    {
        _offset = GUIToGrid(new Vector2(25.5f * DEFAULT_CELL_SIZE, -7 * DEFAULT_CELL_SIZE));
    }

    public void Move(Vector3 dv)
    {
        var x = _offset.x + dv.x * _zoom;
        var y = _offset.y + dv.y * _zoom;
        _offset.x = x;
        _offset.y = y;
    }
    #endregion

    #region service methods
    void DrawLines()
    {
        int lodLevel = (int)(Mathf.Log(_zoom) / 1.5f);
        DrawLODLines(lodLevel > 0 ? lodLevel : 0);
    }

    void DrawLODLines(int level)
    {
        var gridColor = Color.gray;
        var step0 = (int)Mathf.Pow(10, level);
        int halfCount = step0 * CELLS_IN_LINE_COUNT / 2 * 10;
        var length = halfCount * DEFAULT_CELL_SIZE;
        int offsetX = ((int)(_offset.x / DEFAULT_CELL_SIZE)) / (step0 * step0) * step0;
        int offsetY = ((int)(_offset.y / DEFAULT_CELL_SIZE)) / (step0 * step0) * step0;
        for (int i = -halfCount; i <= halfCount; i += step0)
        {
            Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, 0.3f);

            Handles.DrawLine(
                GridToGUI(new Vector2(-length + offsetX * DEFAULT_CELL_SIZE, (i + offsetY) * DEFAULT_CELL_SIZE)),
                GridToGUI(new Vector2(length + offsetX * DEFAULT_CELL_SIZE, (i + offsetY) * DEFAULT_CELL_SIZE))
            );
            Handles.DrawLine(
                GridToGUI(new Vector2((i + offsetX) * DEFAULT_CELL_SIZE, -length + offsetY * DEFAULT_CELL_SIZE)),
                GridToGUI(new Vector2((i + offsetX) * DEFAULT_CELL_SIZE, length + offsetY * DEFAULT_CELL_SIZE))
            );
        }
        offsetX = (offsetX / (10 * step0)) * 10 * step0;
        offsetY = (offsetY / (10 * step0)) * 10 * step0; ;
        for (int i = -halfCount; i <= halfCount; i += step0 * 10)
        {
            Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, 1);
            Handles.DrawLine(
                GridToGUI(new Vector2(-length + offsetX * DEFAULT_CELL_SIZE, (i + offsetY) * DEFAULT_CELL_SIZE)),
                GridToGUI(new Vector2(length + offsetX * DEFAULT_CELL_SIZE, (i + offsetY) * DEFAULT_CELL_SIZE))
            );
            Handles.DrawLine(
                GridToGUI(new Vector2((i + offsetX) * DEFAULT_CELL_SIZE, -length + offsetY * DEFAULT_CELL_SIZE)),
                GridToGUI(new Vector2((i + offsetX) * DEFAULT_CELL_SIZE, length + offsetY * DEFAULT_CELL_SIZE))
            );
        }
    }
    void DrawCenter()
    {
        if (!IsDrawCenter)
            return;

        Handles.color = Color.cyan;
        Handles.DrawLine(
            GridToGUI(Vector3.left * DEFAULT_CELL_SIZE * _zoom),
            GridToGUI(Vector3.right * DEFAULT_CELL_SIZE * _zoom)
        );
        Handles.DrawLine(
            GridToGUI(Vector3.down * DEFAULT_CELL_SIZE * _zoom),
            GridToGUI(Vector3.up * DEFAULT_CELL_SIZE * _zoom)
        );
    }
    #endregion

    #region constructor
    public MiniGameGrid(EditorWindow parentWindow, bool isDrawCenterMark = true)
    {
        _zoom = 7.4f;
        _parentWindow = parentWindow;
        IsDrawCenter = isDrawCenterMark;
        Recenter();
    }

    #endregion

    #region constants
    const float MIN_ZOOM_VALUE = 0.1f;
    const float MAX_ZOOM_VALUE = 1000;
    const int CELLS_IN_LINE_COUNT = 60;
    public const float DEFAULT_CELL_SIZE = 10;
    #endregion
}