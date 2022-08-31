﻿using Grpc.Core;
using Hangfire;
using KianUSA.Application.SeedWork;
using KianUSA.Application.Services.Catalog;
using KianUSA.Application.Services.UpdateDataByExcel;
using System;
using System.IO;
using System.Threading.Tasks;

namespace KianUSA.API.Services
{
    public class ImportDataService : ImportDataSrv.ImportDataSrvBase
    {
        private readonly UpdateCateogryByExcelService CatService;
        private readonly UpdateProductByExcelService PrdService;
        private readonly UpdateRoleByExcelService RlService;
        private readonly UpdateUserByExcelService UsrService;
        private readonly CatalogService CatalogService;
        private readonly Application.Services.Account.AccountService service;
        private readonly IApplicationSettings applicationSettings;
        private readonly IBackgroundJobClient backgroundJobClient;

        public ImportDataService(IApplicationSettings applicationSettings, IBackgroundJobClient backgroundJobClient)
        {
            service = new Application.Services.Account.AccountService();
            CatService = new UpdateCateogryByExcelService();
            PrdService = new UpdateProductByExcelService(applicationSettings);
            RlService = new UpdateRoleByExcelService();
            UsrService = new UpdateUserByExcelService();
            CatalogService = new(applicationSettings);

            this.applicationSettings = applicationSettings;

            this.backgroundJobClient = backgroundJobClient;
        }
        public override async Task<ByFilesResponse> ByFiles(ByFilesRequest request, ServerCallContext context)
        {
            try
            {
                await service.Login(request.Username, request.Password);
                FileStream CatFile = new($"{applicationSettings.ImportPath}Categories.xlsx", FileMode.Open, FileAccess.Read);
                FileStream PrdFile = new($"{applicationSettings.ImportPath}Items.xlsx", FileMode.Open, FileAccess.Read);
                FileStream RoleFile = new($"{applicationSettings.ImportPath}Roles.xlsx", FileMode.Open, FileAccess.Read);
                FileStream UserFile = new($"{applicationSettings.ImportPath}Users.xlsx", FileMode.Open, FileAccess.Read);

                await CatService.Update(CatFile);
                await PrdService.Update(PrdFile);
                await RlService.Update(RoleFile);
                await UsrService.Update(UserFile);


                backgroundJobClient.Enqueue(() => CatalogService.Create());
                return await Task.FromResult(new ByFilesResponse() { IsSuccessful = true });
            }
            catch (Exception Ex)
            {
                return await Task.FromResult(new ByFilesResponse() { IsSuccessful = false, Message = Ex.Message });
            }
        }
    }
}