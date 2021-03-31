using UnityEngine;

public class CCheckProbe
{
    Transform _transform;
    float _radius;

    public Transform Point { get { return _transform; } }
    public float Radius { get { return _radius; } }

    public CCheckProbe(Transform inTransform, float inRadius)
    {
        _transform = inTransform; _radius = inRadius;
    }
}

