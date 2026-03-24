using lib60870;
using lib60870.CS101;
using System;
using System.Collections.Generic;
namespace LYZD.TerminalProtocol
{
    public class EventDatas
    {
        private DateTime _receiveTime;
        public DateTime ReceiveTime
        {
            get { return _receiveTime; }
        }
        private List<EventSignal> _Faults;
        public List<EventSignal> Faults
        {
            get { return _Faults; }
        }
        private List<EventMeasureData> _FaultDatas;
        public List<EventMeasureData> FaultDatas
        {
            get { return _FaultDatas; }
        }
        public EventDatas(EventOfFault fault)
        {
            _receiveTime = DateTime.Now;

            _Faults = new List<EventSignal>();
            foreach (var item in fault.SingleEvents)
            {
                _Faults.Add(new EventSignal(item.ObjectAddress, item.Value, item.Timestamp));
            }

            _FaultDatas = new List<EventMeasureData>();
            if (fault.RemoteMeasureTI == TypeID.M_ME_NA_1)
            {
                foreach (var item in fault.MeasureValues)
                {
                    _FaultDatas.Add(new EventMeasureData(item.ObjectAddress, ((MeasuredValueNormalizedOfFault)item).NormalizedValue, ((MeasuredValueNormalizedOfFault)item).RawValue));
                }
            }
            else if (fault.RemoteMeasureTI == TypeID.M_ME_NC_1)
            {
                foreach (var item in fault.MeasureValues)
                {
                    _FaultDatas.Add(new EventMeasureData(item.ObjectAddress, ((MeasuredValueShortOfFault)item).Value));
                }
            }

        }
    }

    public class EventSignal
    {
        public EventSignal(int signalPoint, bool signalValue, CP56Time2a timestamp)
        {
            SignalPoint = signalPoint;
            SignalValue = signalValue;
            Timestamp = timestamp;
        }
        public int SignalPoint { get; }
        public bool SignalValue { get; }
        public CP56Time2a Timestamp { get; }
    }

    public class EventMeasureData
    {
        public EventMeasureData(int ioa, float value)
        {
            this.IOA = ioa;
            Value = value;
        }
        public EventMeasureData(int ioa, float value, float rawValue)
        {
            this.IOA = ioa;
            Value = value;
            RawValue = rawValue;
        }
        public int IOA { get; }
        public float Value { get; }
        public float RawValue { get; }
    }
}
