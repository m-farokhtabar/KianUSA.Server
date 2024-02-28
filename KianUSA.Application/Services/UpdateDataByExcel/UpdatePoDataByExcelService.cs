using KianUSA.Application.Data;
using KianUSA.Application.Services.UpdateDataByExcel.Helper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace KianUSA.Application.Services.UpdateDataByExcel
{
    public class UpdatePoDataByExcelService
    {
        public async Task Update(Stream stream)
        {
            if (stream is null)
                throw new Exception("Please upload correct PoData excel file.");
            List<Domain.Entity.PoData> newPoData = new();
            List<Domain.Entity.PoDataSecurity> newPoDataSecurity = new();
            try
            {
                var Tables = UpdateByExcelHelper.ReadExcel(stream);
                if (Tables?.Count > 0)
                {
                    if (Tables[0].Rows?.Count > 0)
                    {
                        for (int i = 0; i < Tables[0].Rows.Count; i++)
                        {
                            var Row = Tables[0].Rows[i];
                            Domain.Entity.PoData NewUser = new()
                            {
                                PoNumber = Row["PO Number"].ToString().Trim(),

                                Rep = Row["Rep"].ToString().Trim(),
                                User = Row["User"].ToString().Trim(),
                                Date = Row["Date"].ToString().Trim(),
                                CustomerPO = Row["Customer PO"].ToString().Trim(),
                                EstimateNumber = Row["Estimate Number"].ToString().Trim(),
                                Name = Row["Name"].ToString().Trim(),
                                DueDate = Row["Due Date"].ToString().Trim(),
                                ItemGroup = Row["Item Group"].ToString().Trim(),
                                Forwarder = Row["Forwarder"].ToString().Trim(),
                                IOR = Row["IOR"].ToString().Trim(),
                                ShipTo = Row["Ship To"].ToString().Trim(),
                                ShippingCarrier = Row["Shipping Carrier"].ToString().Trim(),
                                ContainerNumber = Row["Container Number"].ToString().Trim(),
                                ETAAtPort = Row["ETA at Port"].ToString().Trim(),
                            };
                            newPoData.Add(NewUser);
                        }
                    }
                    if (Tables[1].Rows?.Count > 0)
                    {
                        for (int i = 0; i < Tables[1].Rows.Count; i++)
                        {
                            var Row = Tables[1].Rows[i];
                            Domain.Entity.PoDataSecurity NewSecurity = new()
                            {
                                PoNumber = Row["PO Number"].ToString().Trim(),
                                
                                User = Row["User"].ToString().Trim(),
                                Date = Row["Date"].ToString().Trim(),
                                CustomerPO = Row["Customer PO"].ToString().Trim(),
                                EstimateNumber = Row["Estimate Number"].ToString().Trim(),
                                Name = Row["Name"].ToString().Trim(),
                                DueDate = Row["Due Date"].ToString().Trim(),
                                ItemGroup = Row["Item Group"].ToString().Trim(),
                                Forwarder = Row["Forwarder"].ToString().Trim(),
                                IOR = Row["IOR"].ToString().Trim(),
                                ShipTo = Row["Ship To"].ToString().Trim(),
                                ShippingCarrier = Row["Shipping Carrier"].ToString().Trim(),
                                ContainerNumber = Row["Container Number"].ToString().Trim(),
                                ETAAtPort = Row["ETA at Port"].ToString().Trim(),


                                FactoryStatus = Row["Factory Status"].ToString().Trim(),
                                StatusDate = Row["Status Date"].ToString().Trim(),
                                FactoryContainerNumber = Row["Factory Container Number"].ToString().Trim(),
                                FactoryBookingDate = Row["Factory Booking Date"].ToString().Trim(),
                                DocumentsSendOutDate = Row["Doc Send Out Date"].ToString().Trim(),
                                ForwarderName = Row["Forwarder Name"].ToString().Trim(),
                                BookingDate = Row["Booking Date"].ToString().Trim(),
                                Rate = Row["Rate"].ToString().Trim(),
                                ETD = Row["ETD"].ToString().Trim(),
                                ETA = Row["ETA"].ToString().Trim(),
                                PortOfDischarge = Row["Port Of Discharge"].ToString().Trim(),
                                DischargeStatus = Row["Discharge Status"].ToString().Trim(),
                                ShippmentStatus = Row["Shippment Status"].ToString().Trim(),
                                ConfirmDate = Row["Confirm date"].ToString().Trim(),
                                GateIn = Row["Gate In"].ToString().Trim(),

                                EmptyDate = Row["Empty Date"].ToString().Trim(),
                                GateOut = Row["Gate out"].ToString().Trim(),
                                BillDate = Row["Bill Date"].ToString().Trim(),
                                Note = Row["Note"].ToString().Trim()                                
                            };
                            newPoDataSecurity.Add(NewSecurity);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("there are some errors during reading data from PoData excel.[" + ex.Message + "]");
            }
            if (newPoData?.Count > 0)
            {
                try
                {
                    using var db = new Context();
                    var oldPoData = await db.PoDatas.ToListAsync().ConfigureAwait(false);
                    var archivePoData = RecognizeOldDataNeedTobeArchivedOrUpdateOrInsert(oldPoData, newPoData);
                    db.PoDatasArchive.AddRange(archivePoData);
                    if (newPoDataSecurity?.Count > 0)
                    {
                        await db.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"PoDataSecurity\"");
                        db.PoDataSecurity.AddRange(newPoDataSecurity);
                    }
                    await db.SaveChangesAsync();
                }
                catch(Exception ex)
                {
                    throw new Exception($"Cannot update data in database error message is [{ex.Message}]");
                }
            }
            else
                throw new Exception("There is no data in PoData excel file to update!!");

        }

        private List<Domain.Entity.PoDataArchive> RecognizeOldDataNeedTobeArchivedOrUpdateOrInsert(List<Domain.Entity.PoData> oldData, List<Domain.Entity.PoData> newData)
        {
            List<Domain.Entity.PoDataArchive> needTobeArchived = new();
            DateTime CurrentDate = DateTime.Now;
            foreach (var oldItem in oldData)
            {
                var newItem = newData.Find(x => string.Equals(x.PoNumber?.Trim(), oldItem.PoNumber.Trim()));
                //اگر پیدا شد باید به روز شود
                if (newItem is not null)
                {
                    oldItem.Rep = newItem.Rep;
                    oldItem.User = newItem.User;
                    oldItem.Date = newItem.User;
                    oldItem.CustomerPO = newItem.CustomerPO;
                    oldItem.EstimateNumber = newItem.EstimateNumber;
                    oldItem.Name = newItem.Name;
                    oldItem.DueDate = newItem.DueDate;
                    oldItem.ItemGroup = newItem.ItemGroup;
                    oldItem.Forwarder = newItem.Forwarder;
                    oldItem.IOR = newItem.IOR;
                    oldItem.ShipTo = newItem.ShipTo;
                    oldItem.ShippingCarrier = newItem.ShippingCarrier;
                    oldItem.ContainerNumber = newItem.ContainerNumber;
                    oldItem.ETAAtPort = newItem.ETAAtPort;
                    //از لیست حذف می شود چون دیگر نیازی بهش نداریم
                    newData.Remove(newItem);
                }
                //اگر پیدا نشد باید حذف شود و به جدول بایگانی انتقال داده شود
                else
                {
                    needTobeArchived.Add(new Domain.Entity.PoDataArchive()
                    {
                        PoNumber = oldItem.PoNumber,
                        Note = oldItem.Note,
                        Rep = oldItem.Rep,
                        User = oldItem.User,
                        Date = oldItem.Date,
                        CustomerPO = oldItem.CustomerPO,
                        EstimateNumber = oldItem.EstimateNumber,
                        Name = oldItem.Name,                            
                        DueDate = oldItem.DueDate,
                        ItemGroup = oldItem.ItemGroup,
                        Forwarder = oldItem.Forwarder,
                        IOR = oldItem.IOR,
                        ShipTo = oldItem.ShipTo,
                        ShippingCarrier = oldItem.ShippingCarrier,
                        ContainerNumber = oldItem.ContainerNumber,                            
                        BillDate = oldItem.BillDate ?? CurrentDate,
                        BookingDate = oldItem.BookingDate,
                        ConfirmDate = oldItem.ConfirmDate,
                        DischargeStatus = oldItem.DischargeStatus,
                        DocumentsSendOutDate = oldItem.DocumentsSendOutDate,
                        EmptyDate = oldItem.EmptyDate,
                        ETA = oldItem.ETA,
                        ETAAtPort = oldItem.ETAAtPort,
                        ETD = oldItem.ETA,
                        FactoryBookingDate = oldItem.FactoryBookingDate,
                        FactoryContainerNumber = oldItem.FactoryContainerNumber,
                        FactoryStatus = oldItem.FactoryStatus,
                        ForwarderName = oldItem.ForwarderName,
                        GateIn = oldItem.GateIn,
                        GateOut = oldItem.GateOut,
                        PortOfDischarge = oldItem.PortOfDischarge,
                        Rate = oldItem.Rate,
                        ShippmentStatus = oldItem.ShippmentStatus,
                        StatusDate = oldItem.StatusDate
                    });
                }
            }
            //از لیست حذف می شود چون باید به جدول آرشیو منتقل شود و از این جدول حذف شود
            if (needTobeArchived?.Count>0)
                oldData.RemoveAll(x=> needTobeArchived.Any(y=> string.Equals(y.PoNumber,x.PoNumber)));

            //تمام داده های باقی مانده به عنوان داده جدید باید وارد سیستم شود
            oldData.AddRange(newData);
            
            return needTobeArchived;
        }

    }
}
