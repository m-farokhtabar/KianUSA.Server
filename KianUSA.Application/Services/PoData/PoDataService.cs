using KianUSA.Application.Data;
using KianUSA.Domain.Entity;
using KianUSA.Application.SeedWork;
using KianUSA.Application.Services.Account;
using KianUSA.Application.Services.UpdateDataByExcel.Helper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace KianUSA.Application.Services.PoData
{
    public class PoDataService
    {
        private readonly IApplicationSettings appSettings;
        private readonly AuthorizationService Auth;
        public PoDataService(IApplicationSettings appSettings, List<string> userRoles)
        {
            this.appSettings = appSettings;
            Auth = new AuthorizationService(userRoles);
        }

        public (PoDataDto, PoSecurityData) GetDataByExcel()
        {
            PoDataDto Result = null;
            PoSecurityData ColSecurity = null;
            var Tables = GetExcelTables();
            try
            {
                if (Tables?.Count > 0 && Tables[0].Rows?.Count > 0)
                {
                    ColSecurity = GetSecurity(Tables);

                    Result = new PoDataDto
                    {
                        Data = new List<PoExcelDbDataDto>()
                    };
                    if (ColSecurity is not null)
                    {
                        Result.ColumnsHavePermission = new List<ColAccess>();
                        if (ColSecurity.User.HasAccess)
                            Result.ColumnsHavePermission.Add(new ColAccess("User", ColSecurity.User.Writable));
                        if (ColSecurity.Date.HasAccess)
                            Result.ColumnsHavePermission.Add(new ColAccess("Date", ColSecurity.Date.Writable));
                        if (ColSecurity.CustomerPO.HasAccess)
                            Result.ColumnsHavePermission.Add(new ColAccess("Customer PO", ColSecurity.CustomerPO.Writable));
                        if (ColSecurity.EstimateNumber.HasAccess)
                            Result.ColumnsHavePermission.Add(new ColAccess("Estimate Number", ColSecurity.EstimateNumber.Writable));
                        if (ColSecurity.Name.HasAccess)
                            Result.ColumnsHavePermission.Add(new ColAccess("Name", ColSecurity.Name.Writable));
                        //if (ColSecurity.PONumber.HasAccess)
                        //همیشه باید این فیلد باشد چون کلید است
                        Result.ColumnsHavePermission.Add(new ColAccess("PO Number", false));
                        if (ColSecurity.DueDate.HasAccess)
                            Result.ColumnsHavePermission.Add(new ColAccess("Due Date", ColSecurity.DueDate.Writable));

                        if (ColSecurity.ItemGroup.HasAccess)
                            Result.ColumnsHavePermission.Add(new ColAccess("Item Group", ColSecurity.ItemGroup.Writable));
                        if (ColSecurity.Forwarder.HasAccess)
                            Result.ColumnsHavePermission.Add(new ColAccess("Forwarder", ColSecurity.Forwarder.Writable));
                        if (ColSecurity.IOR.HasAccess)
                            Result.ColumnsHavePermission.Add(new ColAccess("IOR", ColSecurity.IOR.Writable));
                        if (ColSecurity.ShipTo.HasAccess)
                            Result.ColumnsHavePermission.Add(new ColAccess("Ship To", ColSecurity.ShipTo.Writable));
                        if (ColSecurity.ShippingCarrier.HasAccess)
                            Result.ColumnsHavePermission.Add(new ColAccess("Shipping Carrier", ColSecurity.ShippingCarrier.Writable));
                        if (ColSecurity.ContainerNumber.HasAccess)
                            Result.ColumnsHavePermission.Add(new ColAccess("Container Number", ColSecurity.ContainerNumber.Writable));
                        if (ColSecurity.ETAAtPort.HasAccess)
                            Result.ColumnsHavePermission.Add(new ColAccess("ETA at Port", ColSecurity.ETAAtPort.Writable));

                        if (ColSecurity.FactoryStatus.HasAccess)
                            Result.ColumnsHavePermission.Add(new ColAccess("Factory Status", ColSecurity.FactoryStatus.Writable));
                        if (ColSecurity.StatusDate.HasAccess)
                            Result.ColumnsHavePermission.Add(new ColAccess("Status Date", ColSecurity.StatusDate.Writable));
                        if (ColSecurity.FactoryContainerNumber.HasAccess)
                            Result.ColumnsHavePermission.Add(new ColAccess("Factory Container Number", ColSecurity.FactoryContainerNumber.Writable));
                        if (ColSecurity.FactoryBookingDate.HasAccess)
                            Result.ColumnsHavePermission.Add(new ColAccess("Factory Booking Date", ColSecurity.FactoryBookingDate.Writable));
                        if (ColSecurity.DocumentsSendOutDate.HasAccess)
                            Result.ColumnsHavePermission.Add(new ColAccess("Doc Send Out Date", ColSecurity.DocumentsSendOutDate.Writable));

                        if (ColSecurity.ForwarderName.HasAccess)
                            Result.ColumnsHavePermission.Add(new ColAccess("Forwarder Name", ColSecurity.ForwarderName.Writable));

                        if (ColSecurity.BookingDate.HasAccess)
                            Result.ColumnsHavePermission.Add(new ColAccess("Booking Date", ColSecurity.BookingDate.Writable));
                        if (ColSecurity.Rate.HasAccess)
                            Result.ColumnsHavePermission.Add(new ColAccess("Rate", ColSecurity.Rate.Writable));
                        if (ColSecurity.ETD.HasAccess)
                            Result.ColumnsHavePermission.Add(new ColAccess("ETD", ColSecurity.ETD.Writable));
                        if (ColSecurity.ETA.HasAccess)
                            Result.ColumnsHavePermission.Add(new ColAccess("ETA", ColSecurity.ETA.Writable));
                        if (ColSecurity.PortOfDischarge.HasAccess)
                            Result.ColumnsHavePermission.Add(new ColAccess("Port Of Discharge", ColSecurity.PortOfDischarge.Writable));
                        if (ColSecurity.DischargeStatus.HasAccess)
                            Result.ColumnsHavePermission.Add(new ColAccess("Discharge Status", ColSecurity.DischargeStatus.Writable));
                        if (ColSecurity.ShippmentStatus.HasAccess)
                            Result.ColumnsHavePermission.Add(new ColAccess("Shippment Status", ColSecurity.ShippmentStatus.Writable));
                        if (ColSecurity.ConfirmDate.HasAccess)
                            Result.ColumnsHavePermission.Add(new ColAccess("Confirm date", ColSecurity.ConfirmDate.Writable));

                        if (ColSecurity.GateIn.HasAccess)
                            Result.ColumnsHavePermission.Add(new ColAccess("Gate In", ColSecurity.GateIn.Writable));
                        if (ColSecurity.EmptyDate.HasAccess)
                            Result.ColumnsHavePermission.Add(new ColAccess("Empty Date", ColSecurity.EmptyDate.Writable));
                        if (ColSecurity.GateOut.HasAccess)
                            Result.ColumnsHavePermission.Add(new ColAccess("Gate out", ColSecurity.GateOut.Writable));
                        if (ColSecurity.BillDate.HasAccess)
                            Result.ColumnsHavePermission.Add(new ColAccess("Bill Date", ColSecurity.BillDate.Writable));
                    }
                    for (int i = 0; i < Tables[0].Rows.Count; i++)
                    {
                        var row = Tables[0].Rows[i];

                        ////به دلیل اینکه در زمان چک کردن نقش تابع نقش خالی را مجوز می دهد به همین خاطر یک چیزی داخلش گذاشتم که این حالت پیش نیاد
                        //string ForwarderNameRole = "[]";
                        //if (string.IsNullOrWhiteSpace(row["Forwarder"].ToString()))
                        //    ForwarderNameRole = "[\"" + row["Forwarder"].ToString().Trim() + "\"]";
                        ////هم نقش های معمول را چک میکند هم فیلد فوروادر را به عنوان نقش چک می کند که ببینید به این سطر دسترسی دارد یا نه
                        //if (Auth.HasUserPermissionToUseData(row["Rep"].ToString().Trim()) || Auth.HasUserPermissionToUseData(ForwarderNameRole))
                        //{
                        var rowData = new PoExcelDbDataDto()
                        {
                            User = ColSecurity.User.HasAccess ? row["User"].ToString().Trim() : null,
                            Date = ColSecurity.Date.HasAccess ? UpdateByExcelHelper.DateTimeToDateString(row["Date"]) : null,
                            CustomerPO = ColSecurity.CustomerPO.HasAccess ? row["Customer PO"].ToString().Trim() : null,
                            EstimateNumber = ColSecurity.EstimateNumber.HasAccess ? row["Estimate Number"].ToString().Trim() : null,
                            Name = ColSecurity.Name.HasAccess ? row["Name"].ToString().Trim() : null,
                            //PONumber = ColSecurity.PONumber.HasAccess ? row["PO Number"].ToString().Trim() : null,
                            PONumber = row["PO Number"].ToString().Trim(),
                            DueDate = ColSecurity.DueDate.HasAccess ? UpdateByExcelHelper.DateTimeToDateString(row["Due Date"]) : null,

                            ItemGroup = ColSecurity.ItemGroup.HasAccess ? row["Item Group"].ToString().Trim() : null,
                            Forwarder = ColSecurity.Forwarder.HasAccess ? row["Forwarder"].ToString().Trim() : null,
                            IOR = ColSecurity.IOR.HasAccess ? row["IOR"].ToString().Trim() : null,
                            //ShipTo = ColSecurity.ShipTo.HasAccess ? row["Ship To"].ToString().Trim() : null,
                            //فعلا دسترسی ستون رو بهش نمی دیم چو بهش نیاز داریم برای سطح دسترسی سطر ها
                            ShipTo = row["Ship To"].ToString().Trim(),
                            ShippingCarrier = ColSecurity.ShippingCarrier.HasAccess ? row["Shipping Carrier"].ToString().Trim() : null,
                            ContainerNumber = ColSecurity.ContainerNumber.HasAccess ? row["Container Number"].ToString().Trim() : null,
                            ETAAtPort = ColSecurity.ContainerNumber.HasAccess ? row["ETA at Port"].ToString().Trim() : null,
                            Rep = row["Rep"].ToString().Trim()
                        };
                        Result.Data.Add(rowData);
                        //}
                    }
                }
            }
            catch (Exception Ex)
            {
                throw new Exception("there are some errors during reading data from excel.");
            }
            return (Result, ColSecurity);
        }
        public async Task FillOutLiveDbDataToPoData(PoDataDto pOData, PoSecurityData ColSecurity)
        {
            using var Db = new Context();
            var models = await Db.PoDatas.AsNoTracking().ToListAsync();
            FillDbColumnsBasedOnPermissions(pOData, models, ColSecurity);

            //if (models?.Count > 0 && pOData.Data?.Count > 0)
            //{
            //    foreach (var data in pOData.Data)
            //    {
            //        var model = models.FirstOrDefault(x => string.Equals(x.PoNumber, data.PONumber, StringComparison.OrdinalIgnoreCase));
            //        if (model is not null)
            //        {
            //            data.FactoryStatus = ColSecurity.FactoryStatus.HasAccess ? model.FactoryStatus : null;
            //            data.StatusDate = ColSecurity.StatusDate.HasAccess ? model.StatusDate : null;
            //            data.FactoryContainerNumber = ColSecurity.FactoryContainerNumber.HasAccess ? model.FactoryContainerNumber : null;
            //            data.FactoryBookingDate = ColSecurity.FactoryBookingDate.HasAccess ? model.FactoryBookingDate : null;
            //            data.DocumentsSendOutDate = ColSecurity.DocumentsSendOutDate.HasAccess ? model.DocumentsSendOutDate : null;


            //            //فعلا دسترسی ستون رو بهش نمی دیم چو بهش نیاز داریم برای سطح دسترسی سطر ها
            //            //data.ForwarderName = ColSecurity.ForwarderName.HasAccess ?  model.ForwarderName : null;
            //            data.ForwarderName = model.ForwarderName;
            //            data.BookingDate = ColSecurity.BookingDate.HasAccess ? model.BookingDate : null;
            //            data.Rate = ColSecurity.Rate.HasAccess ? model.Rate : null;
            //            data.ETD = ColSecurity.ETD.HasAccess ? model.ETD : null;
            //            data.FactoryStatusNeedsToHaveReadyToGO = data.ETD.HasValue && (data.ETD.Value.Subtract(DateTime.Now).TotalDays <= 14);
            //            data.ETA = ColSecurity.ETA.HasAccess ? model.ETA : null;
            //            data.PortOfDischarge = ColSecurity.PortOfDischarge.HasAccess ? model.PortOfDischarge : null;
            //            data.DischargeStatus = ColSecurity.DischargeStatus.HasAccess ? model.DischargeStatus : null;

            //            data.ShippmentStatus = ColSecurity.ShippmentStatus.HasAccess ? model.ShippmentStatus : null;
            //            data.ConfirmDate = ColSecurity.ConfirmDate.HasAccess ? model.ConfirmDate : null;

            //            data.GateIn = ColSecurity.GateIn.HasAccess ? model.GateIn : null;
            //            data.EmptyDate = ColSecurity.EmptyDate.HasAccess ? model.EmptyDate : null;
            //            data.GateOut = ColSecurity.GateOut.HasAccess ? model.GateOut : null;

            //            data.BillDate = ColSecurity.BillDate.HasAccess ? model.BillDate : null;
            //        }

            //    }
            //}
            GetRowsWhichHaveBeAccessedByUser(pOData);
            //if (pOData.Data?.Count > 0)
            //{
            //    foreach (var data in pOData.Data)
            //    {
            //        data.ForwarderName = ColSecurity.ForwarderName.HasAccess ? data.ForwarderName : null;
            //        data.ShipTo = ColSecurity.ShipTo.HasAccess ? data.ShipTo : null;
            //    }
            //}
            CheckPermissionSomeDataAfterCheckingRowPermissions(pOData, ColSecurity);
            pOData.Data = pOData.Data?.Where(x => x.BillDate is null).ToList();
        }
        public async Task FillOutArchiveDbDataToPoData(PoDataDto pOData, PoSecurityData ColSecurity)
        {
            using var Db = new Context();
            var models = await Db.PoDatas.AsNoTracking().Where(x => x.BillDate != null).ToListAsync();
            FillDbColumnsBasedOnPermissions(pOData, models, ColSecurity);
            GetRowsWhichHaveBeAccessedByUser(pOData);
            CheckPermissionSomeDataAfterCheckingRowPermissions(pOData, ColSecurity);
            //به این دلیل دوباره این شرط را میزارم که داده های اکسلی که معادلی در پایگاه داده نداشته اند را با این شرط حذف کند
            pOData.Data = pOData.Data?.Where(x => x.BillDate is not null).ToList();
            //همه دسترسی های نوشتن را غیر فعال می کنیم تا فقط برای نمایش استفاده شود
            foreach (var cPer in pOData.ColumnsHavePermission) 
            {
                cPer.Writable = false;
            }
        }
        private void FillDbColumnsBasedOnPermissions(PoDataDto pOData, List<Domain.Entity.PoData> models, PoSecurityData ColSecurity)
        {
            if (models?.Count > 0 && pOData.Data?.Count > 0)
            {
                foreach (var data in pOData.Data)
                {
                    var model = models.FirstOrDefault(x => string.Equals(x.PoNumber, data.PONumber, StringComparison.OrdinalIgnoreCase));
                    if (model is not null)
                    {
                        data.FactoryStatus = ColSecurity.FactoryStatus.HasAccess ? model.FactoryStatus : null;
                        data.StatusDate = ColSecurity.StatusDate.HasAccess ? model.StatusDate : null;
                        data.FactoryContainerNumber = ColSecurity.FactoryContainerNumber.HasAccess ? model.FactoryContainerNumber : null;
                        data.FactoryBookingDate = ColSecurity.FactoryBookingDate.HasAccess ? model.FactoryBookingDate : null;
                        data.DocumentsSendOutDate = ColSecurity.DocumentsSendOutDate.HasAccess ? model.DocumentsSendOutDate : null;

                        //فعلا دسترسی ستون رو بهش نمی دیم چو بهش نیاز داریم برای سطح دسترسی سطر ها
                        //data.ForwarderName = ColSecurity.ForwarderName.HasAccess ?  model.ForwarderName : null;
                        data.ForwarderName = model.ForwarderName;
                        
                        data.BookingDate = ColSecurity.BookingDate.HasAccess ? model.BookingDate : null;
                        data.Rate = ColSecurity.Rate.HasAccess ? model.Rate : null;
                        data.ETD = ColSecurity.ETD.HasAccess ? model.ETD : null;
                        data.FactoryStatusNeedsToHaveReadyToGO = data.ETD.HasValue && (data.ETD.Value.Subtract(DateTime.Now).TotalDays <= 14);
                        data.ETA = ColSecurity.ETA.HasAccess ? model.ETA : null;
                        data.PortOfDischarge = ColSecurity.PortOfDischarge.HasAccess ? model.PortOfDischarge : null;
                        data.DischargeStatus = ColSecurity.DischargeStatus.HasAccess ? model.DischargeStatus : null;

                        //فعلا دسترسی ستون رو بهش نمی دیم چو بهش نیاز داریم برای سطح دسترسی سطر ها
                        //data.ShippmentStatus = ColSecurity.ShippmentStatus.HasAccess ? model.ShippmentStatus : null;
                        data.ShippmentStatus = model.ShippmentStatus;

                        data.ConfirmDate = ColSecurity.ConfirmDate.HasAccess ? model.ConfirmDate : null;

                        data.GateIn = ColSecurity.GateIn.HasAccess ? model.GateIn : null;
                        data.EmptyDate = ColSecurity.EmptyDate.HasAccess ? model.EmptyDate : null;
                        data.GateOut = ColSecurity.GateOut.HasAccess ? model.GateOut : null;

                        data.BillDate = ColSecurity.BillDate.HasAccess ? model.BillDate : null;
                    }
                }
            }
        }
        /// <summary>
        /// بررسی دسترسی بعضی ستون ها که در مرحله اول نیاز داریم آن را برای دسترسی به سطرها
        /// </summary>
        /// <param name="pOData"></param>
        /// <param name="ColSecurity"></param>
        private void CheckPermissionSomeDataAfterCheckingRowPermissions(PoDataDto pOData, PoSecurityData ColSecurity)
        {
            if (pOData.Data?.Count > 0)
            {
                foreach (var data in pOData.Data)
                {
                    data.ForwarderName = ColSecurity.ForwarderName.HasAccess ? data.ForwarderName : null;
                    data.ShippmentStatus = ColSecurity.ShippmentStatus.HasAccess ? data.ShippmentStatus : null;
                    data.ShipTo = ColSecurity.ShipTo.HasAccess ? data.ShipTo : null;
                }
            }
        }

        public async Task<PoSaveDataResultDto> SaveData(List<Domain.Entity.PoData> data)
        {

            foreach (var item in data)
            {
                if (item.FactoryStatus.HasValue && item.FactoryStatus == FactoryStatus.None)
                    item.FactoryStatus = null;
                
                if (item.ForwarderName.HasValue && item.ForwarderName == ForwarderName.None)
                    item.ForwarderName = null;

                if (item.DischargeStatus.HasValue && item.DischargeStatus == DischargeStatus.None)
                    item.DischargeStatus = null;

                if (item.ShippmentStatus.HasValue && item.ShippmentStatus == ShippmentStatus.None)
                    item.ShippmentStatus = null;
            }

            PoSaveDataResultDto result = null;
            //List<(string poNumber, string role)> PoNumbers = new();
            List<(string PoNumber, string ShipTo, string Rep)> PoNumbers = new();
            PoSecurityData ColSecurity = null;
            var Tables = GetExcelTables();
            if (Tables?.Count > 0 && Tables[0].Rows?.Count > 0)
            {
                ColSecurity = GetSecurity(Tables);
                for (int i = 0; i < Tables[0].Rows.Count; i++)
                {
                    var row = Tables[0].Rows[i];
                    //دقت من عمدا 
                    //ForwarderName.Apex
                    //ShippmentStatus.PleaseAccept
                    //که لیست همه رو بده که بتونیم بعدا واکشی کنیم
                    //و فقط انهایی را بدهد که بر اساس داده های اکسل دسترسی دارد
                    if (HasUserPermissionToThisRow(row["Rep"].ToString().Trim(), ForwarderName.Apex, row["PO Number"].ToString().Trim(), row["Ship To"].ToString().Trim(), ShippmentStatus.PleaseAccept))
                    {
                        //PoNumbers.Add((row["PO Number"].ToString().Trim().ToLower(), row["Rep"].ToString().Trim()));
                        PoNumbers.Add((row["PO Number"].ToString().Trim().ToLower(), row["Ship To"].ToString().Trim(), row["Rep"].ToString().Trim()));
                    }
                }
            }

            using var Db = new Context();
            var DbData = await Db.PoDatas.Where(x => PoNumbers.Select(x => x.PoNumber).ToList().Contains(x.PoNumber)).ToListAsync();
            result = new PoSaveDataResultDto
            {
                Results = new List<PoSaveDataOutput>()
            };
            foreach (var item in data)
            {
                string Message = "";
                bool FactoryStatusNeedsToHaveReadyToGO = false;
                var PoNumber = PoNumbers.FirstOrDefault(x => string.Equals(x.PoNumber, item.PoNumber?.Trim().ToLower()));
                //بررسی اینکه کد رکورد اصلا در اکسل وجود دارد یا نه
                if (!string.IsNullOrWhiteSpace(PoNumber.PoNumber))
                {                    
                    var dbItem = DbData.FirstOrDefault(x => string.Equals(x.PoNumber, item.PoNumber, StringComparison.OrdinalIgnoreCase));
                    if (dbItem is null || dbItem.BillDate == null)
                    {
                        //یعنی رکورد در پایگاه وجو ندارد
                        if (dbItem is null)
                        {
                            //اگر دسترسی داشته باشد دقت شود در این حالت ما
                            //forawarder 
                            //ShippmentStatus
                            //نداریم
                            //چون در حال درج هستیم پس قبلا داده ای نداشتیم که بر اساس آن دو پارامتر بالا را بررسی کنیم
                            if (HasUserPermissionToThisRow(PoNumber.Rep, null, PoNumber.PoNumber, PoNumber.ShipTo, null))
                            {
                                var currentDate = DateTime.Now;
                                item.PoNumber = PoNumber.PoNumber;
                                if (!ColSecurity.FactoryStatus.Writable)
                                {
                                    item.FactoryStatus = null;
                                    item.StatusDate = null;
                                    item.BookingDate = null;
                                }
                                else
                                {
                                    if (item.FactoryStatus != null)
                                        item.StatusDate = currentDate;
                                    if (item.FactoryStatus == FactoryStatus.BookedWithForwarder)
                                        item.BookingDate = currentDate;
                                    //گزینه Ready To Go زمانی بتونی   ثبت کنه که تاریخ فعلی باید  حداقل چهارده روز قبل از etd باشد یعنی اگر 15 روز بود نمی تونه ولی اگر چهارده روز یا سیزده روز بود می تونه
                                    if (item.FactoryStatus == FactoryStatus.ReadyToGo)
                                    {
                                        if (!(item.ETD.HasValue && ColSecurity.ETA.Writable && (item.ETD.Value.Subtract(currentDate).TotalDays <= 14)))
                                        {
                                            Message = "Cannot Accept FactoryStatus As a ReadyToGo because of ETD Date";
                                            item.FactoryStatus = null;
                                        }
                                        else
                                        {
                                            FactoryStatusNeedsToHaveReadyToGO = true;
                                        }
                                    }
                                }
                                //if (!ColSecurity.StatusDate.Writable)
                                //    item.StatusDate = null;
                                if (!ColSecurity.FactoryContainerNumber.Writable)
                                    item.FactoryContainerNumber = null;
                                //if (!ColSecurity.BookingDate.Writable)
                                //    item.BookingDate = null;
                                if (!ColSecurity.DocumentsSendOutDate.Writable)
                                    item.DocumentsSendOutDate = null;
                                if (!ColSecurity.ForwarderName.Writable)
                                    item.ForwarderName = null;
                                if (!ColSecurity.FactoryBookingDate.Writable)
                                    item.FactoryBookingDate = null;
                                if (!ColSecurity.Rate.Writable)
                                    item.Rate = null;
                                if (!ColSecurity.ETD.Writable)
                                    item.ETD = null;
                                else
                                {
                                    if (item.ETD.HasValue && item.ETD.Value.Subtract(currentDate).TotalDays <= 14)
                                        FactoryStatusNeedsToHaveReadyToGO = true;
                                }
                                if (!ColSecurity.ETA.Writable)
                                    item.ETA = null;
                                if (!ColSecurity.PortOfDischarge.Writable)
                                    item.PortOfDischarge = null;
                                if (!ColSecurity.DischargeStatus.Writable)
                                    item.DischargeStatus = null;
                                if (!ColSecurity.ShippmentStatus.Writable)
                                {
                                    item.ShippmentStatus = null;
                                    item.ConfirmDate = null;
                                }
                                else
                                {
                                    if (item.ShippmentStatus == ShippmentStatus.PleaseAccept)
                                        item.ConfirmDate = currentDate;
                                }
                                if (!ColSecurity.GateIn.Writable)
                                    item.GateIn = null;
                                if (!ColSecurity.EmptyDate.Writable)
                                    item.EmptyDate = null;
                                if (!ColSecurity.GateOut.Writable)
                                    item.GateOut = null;
                                if (!ColSecurity.BillDate.Writable)
                                    item.BillDate = null;
                                Db.PoDatas.Add(item);
                                result.Results.Add(new PoSaveDataOutput(item.PoNumber, item.ConfirmDate, item.StatusDate, item.BookingDate, "", FactoryStatusNeedsToHaveReadyToGO, item.Rate));
                            }
                        }
                        //یعنی رکورد قبلا در پایگاه داده بوده
                        else
                        {
                            //دقت شود باید آخرین وضعیت مربوط به
                            //ForwarderName
                            //را که در زمان خواندن است بررسی کرد نه وضعیت جدیدی که کاربر وارد کرده است
                            if (HasUserPermissionToThisRow(PoNumber.Rep, dbItem.ForwarderName, PoNumber.PoNumber, PoNumber.ShipTo, dbItem.ShippmentStatus))
                            {
                                var currentDate = DateTime.Now;
                                if (ColSecurity.FactoryStatus.Writable)
                                {
                                    //اگر وضعیت به حالت بوک بود دیگر نباید تغییر کند
                                    if (!dbItem.FactoryStatus.HasValue || dbItem.FactoryStatus != FactoryStatus.BookedWithForwarder)
                                    {
                                        if (item.FactoryStatus.HasValue)
                                        {
                                            if (dbItem.FactoryStatus != item.FactoryStatus)
                                            {
                                                dbItem.StatusDate = currentDate;
                                                
                                                if (item.FactoryStatus == FactoryStatus.BookedWithForwarder)
                                                {
                                                    dbItem.BookingDate = currentDate;
                                                    dbItem.FactoryStatus = item.FactoryStatus;
                                                }
                                                else
                                                {
                                                    //گزینه Ready To Go زمانی بتونی   ثبت کنه که تاریخ فعلی باید  حداقل چهارده روز قبل از etd باشد یعنی اگر 15 روز بود نمی تونه ولی اگر چهارده روز یا سیزده روز بود می تونه
                                                    if (item.FactoryStatus == FactoryStatus.ReadyToGo)
                                                    {
                                                        if (!(ColSecurity.ETD.Writable && item.ETD.HasValue && (item.ETD.Value.Subtract(currentDate).TotalDays <= 14)))
                                                        {
                                                            Message = "Cannot Accept FactoryStatus As a ReadyToGo because of ETD Date";
                                                        }
                                                        else
                                                        {
                                                            dbItem.FactoryStatus = item.FactoryStatus;
                                                            FactoryStatusNeedsToHaveReadyToGO = true;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        dbItem.FactoryStatus = item.FactoryStatus;
                                                    }
                                                    dbItem.BookingDate = null;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            //برای اینکه هر دفعه با حالت نال دوباره تاریخ مجدد ثبت نکند
                                            if (dbItem.FactoryStatus is not null)
                                                dbItem.StatusDate = currentDate;
                                            dbItem.BookingDate = null;
                                            dbItem.FactoryStatus = item.FactoryStatus;
                                        }

                                    }
                                }
                                //if (ColSecurity.StatusDate.Writable)
                                //    dbItem.StatusDate = item.StatusDate;
                                if (ColSecurity.FactoryContainerNumber.Writable)
                                    dbItem.FactoryContainerNumber = item.FactoryContainerNumber;
                                //if (ColSecurity.BookingDate.Writable)
                                //    dbItem.BookingDate = item.BookingDate;
                                if (ColSecurity.DocumentsSendOutDate.Writable)
                                    dbItem.DocumentsSendOutDate = item.DocumentsSendOutDate;
                                if (ColSecurity.ForwarderName.Writable)
                                    dbItem.ForwarderName = item.ForwarderName;
                                if (ColSecurity.FactoryBookingDate.Writable)
                                    dbItem.FactoryBookingDate = item.FactoryBookingDate;

                                //در صورتی که ریت مجوز نوشتن نداشت و یانکه وضعیت برابر 0 بود و امکان نوشتن مجوز هم نبود و یا اینکه مجوز تغییر وضعیت بود ولی هم در پایگاه داده و داده ارسالی هر دو 0 بود
                                if (!(
                                    !ColSecurity.Rate.Writable || 
                                    (dbItem.ShippmentStatus == 0 && !ColSecurity.ShippmentStatus.Writable) ||
                                    (dbItem.ShippmentStatus == 0 && item.ShippmentStatus == 0 && ColSecurity.ShippmentStatus.Writable)
                                    ))
                                {
                                    dbItem.Rate = item.Rate;
                                }
                                    //اگر وضعیت قابل تعییر نیست بر اساس آخرین تغییر ذخیره شده وضعیت را بررسی می کنیم
                                //if (ColSecurity.Rate.Writable && (dbItem.ShippmentStatus != 0 && !ColSecurity.ShippmentStatus.Writable))
                                //    dbItem.Rate = item.Rate;

                                //if (ColSecurity.Rate.Writable && (dbItem.ShippmentStatus != 0 && item.ShippmentStatus != 0 && ColSecurity.ShippmentStatus.Writable))
                                //    dbItem.Rate = item.Rate;
                                if (ColSecurity.ETD.Writable)
                                {
                                    dbItem.ETD = item.ETD;
                                    if (item.ETD.HasValue && item.ETD.Value.Subtract(currentDate).TotalDays <= 14)
                                        FactoryStatusNeedsToHaveReadyToGO = true;
                                }
                                if (ColSecurity.ETA.Writable)
                                    dbItem.ETA = item.ETA;
                                if (ColSecurity.PortOfDischarge.Writable)
                                    dbItem.PortOfDischarge = item.PortOfDischarge;
                                if (ColSecurity.DischargeStatus.Writable)
                                    dbItem.DischargeStatus = item.DischargeStatus;
                                if (ColSecurity.ShippmentStatus.Writable)
                                {
                                    //یعنی قبلا چیزی برای وضعیت ترانزیت مشخص نشده بود
                                    if (!dbItem.ShippmentStatus.HasValue)
                                    {
                                        //اگر قبلا چیزی وارد نشده بود و حالا نوع انتقال رو در حالت پذیرفتن زده 
                                        if (item.ShippmentStatus.HasValue && item.ShippmentStatus == ShippmentStatus.PleaseAccept)
                                            dbItem.ConfirmDate = currentDate;
                                        else
                                            dbItem.ConfirmDate = null;
                                    }
                                    //وضعیت ترازیت قبلا مقدار داشته
                                    else
                                    {
                                        //وضعیت ترانزیت رو کاربر مشخص نکرده
                                        if (!item.ShippmentStatus.HasValue)
                                            dbItem.ConfirmDate = null;
                                        //وضعیت را کاربر مشخص کرده و حالت پذیرفتن نیست
                                        else if (item.ShippmentStatus != ShippmentStatus.PleaseAccept)
                                            dbItem.ConfirmDate = null;
                                        //وضعیت را کاربر مشخص کرده و حالت پذیرفتن است و  در پایگاه داده هم قبلا وضعیت پذیرفتن ثبت نشده است
                                        else if (dbItem.ShippmentStatus != ShippmentStatus.PleaseAccept)
                                            dbItem.ConfirmDate = currentDate;
                                    }
                                    dbItem.ShippmentStatus = item.ShippmentStatus;
                                }
                                if (ColSecurity.GateIn.Writable)
                                    dbItem.GateIn = item.GateIn;
                                if (ColSecurity.EmptyDate.Writable)
                                    dbItem.EmptyDate = item.EmptyDate;
                                if (ColSecurity.GateOut.Writable)
                                    dbItem.GateOut = item.GateOut;
                                if (ColSecurity.BillDate.Writable)
                                    dbItem.BillDate = item.BillDate;

                                result.Results.Add(new PoSaveDataOutput(dbItem.PoNumber, dbItem.ConfirmDate, dbItem.StatusDate, dbItem.BookingDate, Message, FactoryStatusNeedsToHaveReadyToGO, dbItem.Rate));
                            }
                        }
                    }
                }
            }
            await Db.SaveChangesAsync();
            return result;
        }

        /// <summary>
        /// این متد مشخص می کند که کاربر به کدام سطر دسترسی دارد و به کدام ندارد
        /// </summary>
        /// <param name="pOData"></param>
        private void GetRowsWhichHaveBeAccessedByUser(PoDataDto pOData)
        {
            List<PoExcelDbDataDto> AuthorizedRows = new();            
            foreach (var data in pOData.Data)
            {                
                if (HasUserPermissionToThisRow(data.Rep, data.ForwarderName, data.PONumber, data.ShipTo,data.ShippmentStatus))
                    AuthorizedRows.Add(data);
            }
            if (AuthorizedRows.Count > 0)
                pOData.Data = AuthorizedRows;
            else
                pOData.Data = null;
        }
        private bool HasUserPermissionToThisRow(string Rep, ForwarderName? ForwarderName, string PONumber, string ShipTo, ShippmentStatus? ShippmentStatus)
        {
            //به دلیل اینکه در زمان چک کردن نقش تابع نقش خالی را مجوز می دهد به همین خاطر یک چیزی داخلش گذاشتم که این حالت پیش نیاد
            string PONumberRole = "[]";
            if (PONumber.StartsWith("LY", StringComparison.OrdinalIgnoreCase) ||
                PONumber.StartsWith("KF", StringComparison.OrdinalIgnoreCase) ||
                PONumber.StartsWith("MH", StringComparison.OrdinalIgnoreCase) ||
                PONumber.StartsWith("DB", StringComparison.OrdinalIgnoreCase))
            {
                PONumberRole = "[\"" + PONumber[0..2] + " Factory\"]";
            }
            //به دلیل اینکه در زمان چک کردن نقش تابع نقش خالی را مجوز می دهد به همین خاطر یک چیزی داخلش گذاشتم که این حالت پیش نیاد
            string ShipToRole = "[]";
            if (!string.IsNullOrWhiteSpace(ShipTo))
            {
                ShipToRole = "[";
                if (ShipTo.StartsWith("KIAN USA", StringComparison.OrdinalIgnoreCase))
                    ShipToRole += "\"Harmun_Trucking\",\"KIAN_Employee_WH\"";
                if (ShipTo.StartsWith("Sacramento WH", StringComparison.OrdinalIgnoreCase) && ShippmentStatus.HasValue && ShippmentStatus == Domain.Entity.ShippmentStatus.PleaseAccept)
                {
                    ShipToRole += (ShipToRole == "[") ? "" : ",";
                    ShipToRole += "\"Check_ETA\"";
                }
                ShipToRole += "]";
            }

            if (Auth.HasUserPermissionToUseData(Rep) ||
                HasUserPermissionByCheckForwarderNameToThisRow(ForwarderName) || Auth.HasUserPermissionToUseData(PONumberRole) || Auth.HasUserPermissionToUseData(ShipToRole))
            {
                return true;
            }
            return false;
        }
        private bool HasUserPermissionByCheckForwarderNameToThisRow(ForwarderName? ForwarderName)
        {
            //به دلیل اینکه در زمان چک کردن نقش تابع نقش خالی را مجوز می دهد به همین خاطر یک چیزی داخلش گذاشتم که این حالت پیش نیاد
            string ForwarderNameRole = "[]";
            if (ForwarderName.HasValue)
                ForwarderNameRole = "[\"" + Enum.GetName(typeof(ForwarderName), ForwarderName) + "\"]";

            return Auth.HasUserPermissionToUseData(ForwarderNameRole);
        }
        private PoSecurityData GetSecurity(DataTableCollection Tables)
        {
            PoSecurityData ColSecurity = null;
            if (Tables?.Count > 1 && Tables[1].Rows?.Count > 0)
            {
                var row = Tables[1].Rows[0];
                DataRow writePermissionRow = null;
                if (Tables[1].Rows?.Count > 1)
                    writePermissionRow = Tables[1].Rows[1];
                ColSecurity = new PoSecurityData()
                {
                    User = new ColPermission(row["User"].ToString().Trim(), writePermissionRow["User"].ToString().Trim(), Auth),
                    Date = new ColPermission(row["Date"].ToString().Trim(), writePermissionRow["Date"].ToString().Trim(), Auth),
                    CustomerPO = new ColPermission(row["Customer PO"].ToString().Trim(), writePermissionRow["Customer PO"].ToString().Trim(), Auth),
                    EstimateNumber = new ColPermission(row["Estimate Number"].ToString().Trim(), writePermissionRow["Estimate Number"].ToString().Trim(), Auth),
                    Name = new ColPermission(row["Name"].ToString().Trim(), writePermissionRow["Name"].ToString().Trim(), Auth),
                    PONumber = new ColPermission(row["PO Number"].ToString().Trim(), writePermissionRow["PO Number"].ToString().Trim(), Auth),
                    DueDate = new ColPermission(row["Due Date"].ToString().Trim(), writePermissionRow["Due Date"].ToString().Trim(), Auth),

                    ItemGroup = new ColPermission(row["Item Group"].ToString().Trim(), writePermissionRow["Item Group"].ToString().Trim(), Auth),
                    Forwarder = new ColPermission(row["Forwarder"].ToString().Trim(), writePermissionRow["Forwarder"].ToString().Trim(), Auth),
                    IOR = new ColPermission(row["IOR"].ToString().Trim(), writePermissionRow["IOR"].ToString().Trim(), Auth),
                    ShipTo = new ColPermission(row["Ship To"].ToString().Trim(), writePermissionRow["Ship To"].ToString().Trim(), Auth),
                    ShippingCarrier = new ColPermission(row["Shipping Carrier"].ToString().Trim(), writePermissionRow["Shipping Carrier"].ToString().Trim(), Auth),
                    ContainerNumber = new ColPermission(row["Container Number"].ToString().Trim(), writePermissionRow["Container Number"].ToString().Trim(), Auth),
                    ETAAtPort = new ColPermission(row["ETA at Port"].ToString().Trim(), writePermissionRow["ETA at Port"].ToString().Trim(), Auth),

                    FactoryStatus = new ColPermission(row["Factory Status"].ToString().Trim(), writePermissionRow["Factory Status"].ToString().Trim(), Auth),
                    StatusDate = new ColPermission(row["Status Date"].ToString().Trim(), writePermissionRow["Status Date"].ToString().Trim(), Auth),
                    FactoryContainerNumber = new ColPermission(row["Factory Container Number"].ToString().Trim(), writePermissionRow["Factory Container Number"].ToString().Trim(), Auth),
                    FactoryBookingDate = new ColPermission(row["Factory Booking Date"].ToString().Trim(), writePermissionRow["Factory Booking Date"].ToString().Trim(), Auth),
                    DocumentsSendOutDate = new ColPermission(row["Doc Send Out Date"].ToString().Trim(), writePermissionRow["Doc Send Out Date"].ToString().Trim(), Auth),

                    ForwarderName = new ColPermission(row["Forwarder Name"].ToString().Trim(), writePermissionRow["Forwarder Name"].ToString().Trim(), Auth),
                    BookingDate = new ColPermission(row["Booking Date"].ToString().Trim(), writePermissionRow["Booking Date"].ToString().Trim(), Auth),
                    Rate = new ColPermission(row["Rate"].ToString().Trim(), writePermissionRow["Rate"].ToString().Trim(), Auth),
                    ETD = new ColPermission(row["ETD"].ToString().Trim(), writePermissionRow["ETD"].ToString().Trim(), Auth),
                    ETA = new ColPermission(row["ETA"].ToString().Trim(), writePermissionRow["ETA"].ToString().Trim(), Auth),
                    PortOfDischarge = new ColPermission(row["Port Of Discharge"].ToString().Trim(), writePermissionRow["Port Of Discharge"].ToString().Trim(), Auth),
                    DischargeStatus = new ColPermission(row["Discharge Status"].ToString().Trim(), writePermissionRow["Discharge Status"].ToString().Trim(), Auth),
                    ShippmentStatus = new ColPermission(row["Shippment Status"].ToString().Trim(), writePermissionRow["Shippment Status"].ToString().Trim(), Auth),
                    ConfirmDate = new ColPermission(row["Confirm date"].ToString().Trim(), writePermissionRow["Confirm date"].ToString().Trim(), Auth),
                    GateIn = new ColPermission(row["Gate In"].ToString().Trim(), writePermissionRow["Gate In"].ToString().Trim(), Auth),
                    EmptyDate = new ColPermission(row["Empty Date"].ToString().Trim(), writePermissionRow["Empty Date"].ToString().Trim(), Auth),
                    GateOut = new ColPermission(row["Gate out"].ToString().Trim(), writePermissionRow["Gate out"].ToString().Trim(), Auth),
                    BillDate = new ColPermission(row["Bill Date"].ToString().Trim(), writePermissionRow["Bill Date"].ToString().Trim(), Auth)
                };
            }

            return ColSecurity;
        }
        private DataTableCollection GetExcelTables()
        {
            try
            {
                FileStream excelDataStream = new(appSettings.PoDataPath, FileMode.Open, FileAccess.Read);
                if (excelDataStream is null)
                    throw new Exception("I counld not find any excel files!");
                return UpdateByExcelHelper.ReadExcel(excelDataStream);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
