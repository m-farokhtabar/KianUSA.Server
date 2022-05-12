using ExcelDataReader;
using KianUSA.Application.Services.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace KianUSA.Application.Services.UpdateDataByExcel.Helper
{
    public static class UpdateByExcelHelper
    {
        public static DataTableCollection ReadExcel(Stream stream)
        {
            try
            {
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                IExcelDataReader reader = ExcelReaderFactory.CreateReader(stream);
                DataSet result = reader.AsDataSet(new ExcelDataSetConfiguration()
                {
                    ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration()
                    {
                        UseHeaderRow = true
                    }
                });
                reader.Close();
                return result.Tables;
            }
            catch
            {
                return null;
            }
        }
        public static int? GetInt32(object Value)
        {
            if (Value is null)
                return null;
            string ValueString = Value.ToString();
            if (string.IsNullOrWhiteSpace(ValueString))
                return null;
            try
            {
                return Convert.ToInt32(ValueString);
            }
            catch
            {
                return null;
            }

        }
        public static double? GetDouble(object Value)
        {
            if (Value is null)
                return null;
            string ValueString = Value.ToString();
            if (string.IsNullOrWhiteSpace(ValueString))
                return null;
            try
            {
                return Math.Round(Convert.ToDouble(ValueString), 2, MidpointRounding.ToZero);
            }
            catch
            {
                return null;
            }

        }
        public static decimal? GetDecimal(object Value)
        {
            if (Value is null)
                return null;
            string ValueString = Value.ToString();
            if (string.IsNullOrWhiteSpace(ValueString))
                return null;
            try
            {
                return Convert.ToDecimal(ValueString);
            }
            catch
            {
                return null;
            }
        }
        public static string GenerateSlug(string Name, object Slug, List<string> Slugs)
        {
            int SlugIndex = 1;
            string RealSlug = Slug is null || string.IsNullOrWhiteSpace(Slug.ToString()) ? Name.Slugify() : Slug.ToString().Slugify();
            while (Slugs.Find(x => x.Equals(RealSlug)) is not null)
            {
                RealSlug = $"{RealSlug}_{SlugIndex}";
                SlugIndex++;
            }
            return RealSlug;
        }

    }
}
