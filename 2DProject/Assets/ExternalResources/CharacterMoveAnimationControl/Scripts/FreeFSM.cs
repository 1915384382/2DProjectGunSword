using System;
using System.Collections.Generic;

namespace FreeFSM
{
    public enum ESwitchResult
    {
        Passed, //switch was successful
        CloseSwitch, //this switch closed
        SwitchingTime, //now too fast switch
        AlreadyInThisState,
        SwitchBlocked //any switch closed
    }

    public delegate double DGetTimeNow();

    //The FSM allows switch between any states. Except special closed.
    //The FSM allows to set switch time. In this time closed other switch.
    //The FSM allows to block any switch.

    public class CFreeFSMbase<TState, TSwitchPackage>
    {
        public class SwitchEventArgs<TPackage> : EventArgs
        {
            private readonly KeyValuePair<TState, TState> _data;
            private readonly TPackage _package;

            public TPackage Package { get { return _package; } }
            public KeyValuePair<TState, TState> Switch { get { return _data; } }

            public SwitchEventArgs(TState from, TState to, TPackage inPackage)
            {
                _package = inPackage;
                _data = new KeyValuePair<TState, TState>(from, to);
            }

            public override string ToString()
            {
                return string.Format("{0}->{1}", _data.Key, _data.Value);
            }
        }
        public event EventHandler<SwitchEventArgs<TSwitchPackage>> OnSwitch;

        TState _state; //current state

        //for switch time
        double _lastswitch;
        TimeSpan _switch_stan_period;

        DGetTimeNow _GetTimeNow;

        bool _switchbloked = false;

        List<KeyValuePair<TState, TState>> _closetransition = new List<KeyValuePair<TState, TState>>();

        public bool IsStan { get { return _GetTimeNow() - _lastswitch < _switch_stan_period.TotalSeconds; } }
        public TState State { get { return _state; } }

        public CFreeFSMbase(TState initstate, DGetTimeNow inGetTimeNow, TimeSpan switch_stan_period)
        {
            if (inGetTimeNow != null)
                _GetTimeNow = inGetTimeNow;
            else
                _GetTimeNow = () => { return (DateTime.UtcNow - DateTime.MinValue).TotalSeconds; };

            _state = initstate;
            _lastswitch = _GetTimeNow();
            _switch_stan_period = switch_stan_period;
        }

        public CFreeFSMbase(TState initstate) : this(initstate, null, TimeSpan.FromMilliseconds(0)) { }

        public ESwitchResult Switch(TState inNewState, TSwitchPackage inPackage = default(TSwitchPackage))
        {
            if (inNewState.Equals(_state))
                return ESwitchResult.AlreadyInThisState;

            if (!IsOpenSwitch(inNewState))
                return ESwitchResult.CloseSwitch;

            if (IsStan)
                return ESwitchResult.SwitchingTime;

            if (_switchbloked)
                return ESwitchResult.SwitchBlocked;

            TState old = _state;
            _state = inNewState;
            _lastswitch = _GetTimeNow();

            var handler = OnSwitch;
            if (handler != null) handler(this, new SwitchEventArgs<TSwitchPackage>(old, _state, inPackage));

            return ESwitchResult.Passed;
        }


        public void SetAnySwitchBlocked(bool v) { _switchbloked = v; }
        public bool IsSwitchBlocked() { return _switchbloked; }

        public void RestartState()
        {
            _lastswitch = _GetTimeNow();

            var handler = OnSwitch;
            if (handler != null) handler(this, new SwitchEventArgs<TSwitchPackage>(_state, _state, default(TSwitchPackage)));
        }


        public bool IsOpenSwitch(TState inNewState)
        {
            if (_closetransition.FindIndex(p => p.Key.Equals(_state) && p.Value.Equals(inNewState)) != -1)
                return false;
            return true;
        }

        public void CloseSwitch(TState from, TState to)
        {
            if (_closetransition.FindIndex(p => p.Key.Equals(from) && p.Value.Equals(to)) != -1)
                return;
            _closetransition.Add(new KeyValuePair<TState, TState>(from, to));
        }
    }

    public class CFreeFSM<TState>: CFreeFSMbase<TState, int>
    {
        public CFreeFSM(TState initstate, DGetTimeNow inGetTimeNow, TimeSpan switch_stan_period): base(initstate, inGetTimeNow, switch_stan_period) { }

        public CFreeFSM(TState initstate) : this(initstate, null, TimeSpan.FromMilliseconds(0)) { }

        public ESwitchResult Switch(TState inNewState)
        {
            return base.Switch(inNewState);
        }
    }
}
