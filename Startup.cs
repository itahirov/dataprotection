using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.DataProtection;
using Azure.Identity;
using Azure.Extensions.AspNetCore.DataProtection.Blobs;
using Azure.Core;
using Azure.Storage.Blobs;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;

namespace MVCAspNetCore
{
    public class Startup
    {
        private readonly AzureServiceTokenProvider _tokenProvider;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            _tokenProvider = new AzureServiceTokenProvider();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var cnstr="DefaultEndpointsProtocol=https;AccountName=storageaccountistaha15f;AccountKey=jQ1Ks3JJph1OigNWqersT8CTkVDbBiT7oz1hyTF25IoajfJuXwHUz6JQld8AvaqyrNSHPpN7XVakp75bJ8AOGg==;EndpointSuffix=core.windows.net";
            var blobClient= new BlobClient(cnstr,"dataprotection","key9.xml");
            var kvUri= "https://istahisqlkv.vault.azure.net/keys/dp-key1/b032ed9bc60d44fcb132b17750d40273";
            var kvClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(_tokenProvider.KeyVaultTokenCallback));
            services.AddDataProtection()
            .PersistKeysToAzureBlobStorage(blobClient);
            //.ProtectKeysWithAzureKeyVault(kvClient,kvUri);
            /*services.AddDataProtection() 
            .PersistKeysToAzureBlobStorage(new Uri("<blobUriWithSasToken>")) 
            .ProtectKeysWithAzureKeyVault("<keyIdentifier>", "<clientId>", "<clientSecret>");*/
            var provider = new EphemeralDataProtectionProvider();
            var protector = provider.CreateProtector("prt1");
            Console.WriteLine(protector.Protect("test"));
            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
