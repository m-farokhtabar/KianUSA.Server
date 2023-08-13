using System;

namespace KianUSA.Application.Entity
{
    public class PoData
    {
        public string PoNumber { get; set; }
        //Factory
        public FactoryStatus? FactoryStatus { get; set; }
        public DateTime? StatusDate { get; set; }
        public string FactoryContainerNumber { get; set; }
        public DateTime? BookingDate { get; set; }
        public DateTime? DocumentsSendOutDate { get; set; }
        //Forwarder
        public ForwarderName? ForwarderName { get; set; }
        public DateTime? FactoryBookingDate { get; set; }
        public double? Rate { get; set; }
        public DateTime? ETD { get; set; }
        public DateTime? ETA { get; set; }
        public string PortOfDischarge { get; set; }
        public DischargeStatus? DischargeStatus { get; set; }
        //Shawn
        public ShippmentStatus? ShippmentStatus { get; set; }
        public DateTime? ConfirmDate { get; set; }
        //Warehouse Record
        public DateTime? GateIn { get; set; }
        public DateTime? EmptyDate { get; set; }
        public DateTime? GateOut { get; set; }
        //Ap
        public DateTime? BillDate { get; set; }
    }
    public enum FactoryStatus
    {
        NotStarted,
        WaitingForFabric,
        InProduction,
        ReadyToGo,
        WaitingForConfirmation,
        BookedWithForwarder,
        Canceled
    }
    public enum ForwarderName
    {
        Apex,
        Hecny,
        Other,
        OEC,
        Hold
    }
    public enum DischargeStatus
    {
        NotArrived,
        OnVassel,
        ReadyForPickUp,
        OnHold
    }
    public enum ShippmentStatus
    {
        PleaseAccept,
        DoNotAccept,
        HoldTheBooking,
        Change
    }
}
