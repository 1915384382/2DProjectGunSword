using System;

public struct ProtectInt
{
    public static Action actionZuoBi;
    public static int randomValue1;
    public static int randomValue2 = 0xf7ef7ef;
    public static int randomOrder;
    public static int randomOrderValue;

    int hashOrder;
    int hashValue1;
    int hashValue2;

    int NowValue {
        get {
            int order = hashOrder ^ randomOrderValue;
            int now1 = hashValue1 ^ randomValue1;
            int now2 = hashValue2 ^ randomValue2;
            if (now1 + order != now2) {
                OnProtectFun();
                return 0;
            }
            return now1;
        }
        set {
            if (randomOrder + 1 >= int.MaxValue)
                randomOrder = 0;
            else
                randomOrder++;
            hashOrder = randomOrder ^ randomOrderValue;
            hashValue1 = value ^ randomValue1;
            hashValue2 = (value + randomOrder) ^ randomValue2;
        }
    }

    public static implicit operator ProtectInt(int _value)
    {
        return new ProtectInt() { NowValue = _value };
    }

    public static implicit operator int(ProtectInt _value)
    {
        return _value.NowValue;
    }

    public static bool operator ==(ProtectInt _now, int _value)
    {
        return _now.NowValue == _value;
    }

    public static bool operator !=(ProtectInt _now, int _value)
    {
        return _now.NowValue != _value;
    }

    public static bool operator >(ProtectInt _now, int _value)
    {
        return _now.NowValue > _value;
    }

    public static bool operator >=(ProtectInt _now, int _value)
    {
        return _now.NowValue >= _value;
    }

    public static bool operator <(ProtectInt _now, int _value)
    {
        return _now.NowValue < _value;
    }

    public static bool operator <=(ProtectInt _now, int _value)
    {
        return _now.NowValue <= _value;
    }

    public static ProtectInt operator +(ProtectInt _now, int _value)
    {
        return new ProtectInt() { NowValue = _now.NowValue + _value };
    }

    public static ProtectInt operator -(ProtectInt _now, int _value)
    {
        return new ProtectInt() { NowValue = _now.NowValue - _value };
    }

    public static ProtectInt operator *(ProtectInt _now, int _value)
    {
        return new ProtectInt() { NowValue = _now.NowValue * _value };
    }

    public static ProtectInt operator /(ProtectInt _now, int _value)
    {
        return new ProtectInt() { NowValue = _now.NowValue / _value };
    }

    public static ProtectInt operator %(ProtectInt _now, int _value)
    {
        return new ProtectInt() { NowValue = _now.NowValue % _value };
    }

    public static ProtectInt operator &(ProtectInt _now, int _value)
    {
        return new ProtectInt() { NowValue = _now.NowValue & _value };
    }

    public static ProtectInt operator |(ProtectInt _now, int _value)
    {
        return new ProtectInt() { NowValue = _now.NowValue | _value };
    }

    public static ProtectInt operator ^(ProtectInt _now, int _value)
    {
        return new ProtectInt() { NowValue = _now.NowValue ^ _value };
    }

    public static bool operator ==(ProtectInt _now, ProtectInt _value)
    {
        return _now.NowValue == _value.NowValue;
    }

    public static bool operator !=(ProtectInt _now, ProtectInt _value)
    {
        return _now.NowValue != _value.NowValue;
    }

    public static bool operator >(ProtectInt _now, ProtectInt _value)
    {
        return _now.NowValue > _value.NowValue;
    }

    public static bool operator >=(ProtectInt _now, ProtectInt _value)
    {
        return _now.NowValue >= _value.NowValue;
    }

    public static bool operator <(ProtectInt _now, ProtectInt _value)
    {
        return _now.NowValue < _value.NowValue;
    }

    public static bool operator <=(ProtectInt _now, ProtectInt _value)
    {
        return _now.NowValue <= _value.NowValue;
    }

    public static ProtectInt operator +(ProtectInt _now, ProtectInt _value)
    {
        return new ProtectInt() { NowValue = _now.NowValue + _value.NowValue };
    }

    public static ProtectInt operator -(ProtectInt _now, ProtectInt _value)
    {
        return new ProtectInt() { NowValue = _now.NowValue - _value.NowValue };
    }

    public static ProtectInt operator *(ProtectInt _now, ProtectInt _value)
    {
        return new ProtectInt() { NowValue = _now.NowValue * _value.NowValue };
    }

    public static ProtectInt operator /(ProtectInt _now, ProtectInt _value)
    {
        return new ProtectInt() { NowValue = _now.NowValue / _value.NowValue };
    }

    public static ProtectInt operator ++(ProtectInt _now)
    {
        return new ProtectInt() { NowValue = _now.NowValue + 1 };
    }

    public static ProtectInt operator --(ProtectInt _now)
    {
        return new ProtectInt() { NowValue = _now.NowValue - 1 };
    }

    public override int GetHashCode()
    {
        return NowValue;
    }

    public override bool Equals(object obj)
    {
        return NowValue == obj.GetHashCode();
    }

    public void OnProtectFun()
    {
        if (hashValue1 != 0 || hashValue2 != 0)
        {
            if (actionZuoBi != null)
                actionZuoBi();
        }
    }

    public override string ToString()
    {
        return NowValue.ToString();
    }
}
