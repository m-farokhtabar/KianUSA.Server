using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KianUSA.Application.Services.PoData
{
    public class PoSaveDataResultDto
    {
        public List<PoSaveDataOutput> Results { get; set; }
    }
    public class PoSaveDataOutput
    {
        public PoSaveDataOutput()
        {

        }

        public PoSaveDataOutput(string poNumber, DateTime? confirmDate, DateTime? statusDate, DateTime? bookingDate, string message)
        {
            PoNumber = poNumber;
            ConfirmDate = confirmDate;
            StatusDate = statusDate;
            BookingDate = bookingDate;
            Message = message;
        }

        public string PoNumber { get; set; }
        public DateTime? ConfirmDate { get; set; }
        public DateTime? StatusDate { get; set; }
        public DateTime? BookingDate { get; set; }
        public string Message { get; set; }

    }

}
