using UnityEngine;

public class MimicGizmos : MonoBehaviour
{
    [SerializeField] private Transform _zeroPoint;
    [SerializeField] private float _radius;
    [SerializeField] private float _angle;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(_zeroPoint.position, FindPoint(_angle));
        Gizmos.color = Color.red;
        Gizmos.DrawLine(_zeroPoint.position, FindPoint(-_angle));
    }

    private Vector2 FindPoint(float angle)
    {
        return new Vector2(_zeroPoint.position.x + _radius * Mathf.Cos((270 +  angle) * Mathf.Deg2Rad), _zeroPoint.position.y + _radius * Mathf.Sin((270 + angle) * Mathf.Deg2Rad));
    }
}