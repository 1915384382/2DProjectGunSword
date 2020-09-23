using UnityEngine;
using System.Collections;

public class CScalarVelocityIntegrator
{
    private float _fCurVelocity;
    private float _fAccel;
    private float _fBreake;

    public float Velocity { get { return _fCurVelocity; } }

    public CScalarVelocityIntegrator(float inCurVelocity, float inAccel, float inBrake)
    {
        _fCurVelocity = inCurVelocity;
        _fAccel = inAccel;
        _fBreake = inBrake;
    }

    public float Update(float in_fDeltaTime, float in_fCommandVelocity)
    {
        float accel = _fAccel;
        if(Mathf.Abs(in_fCommandVelocity) < Mathf.Abs(_fCurVelocity))
            accel = _fBreake;

        float fDelta = in_fCommandVelocity - _fCurVelocity;
        if (Mathf.Abs(fDelta) < accel * in_fDeltaTime)
            _fCurVelocity = in_fCommandVelocity;
        else
            _fCurVelocity += in_fDeltaTime * accel * Mathf.Sign(fDelta);
        return _fCurVelocity;
    }

    public void SetNewAccel(float inAccel) { _fAccel = inAccel; }
    public void SetNewBrake(float inBrake) { _fBreake = inBrake; }

    public void SetNewAccel(float inMaxVelocity, float inDestinationTime)
    {
        _fAccel = inMaxVelocity / Mathf.Max(inDestinationTime, 0.00001f);
    }

    public void SetNewVelocity(float in_fVelocity) { _fCurVelocity = in_fVelocity; }
}

