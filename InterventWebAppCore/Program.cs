using Intervent.DAL;
using InterventWebApp;
using InterventWebApp.Helpers;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<InterventDatabase>
    (options =>
    {
        options.UseSqlServer(builder.Configuration["ConnectionStrings:DefaultConnection"]);
    });
builder.Services.AddIdentity<ApplicationUser, IdentityRole<int>>(options =>
    {
        options.SignIn.RequireConfirmedAccount = true;
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 8;
    }).AddEntityFrameworkStores<InterventDatabase>()
    .AddDefaultTokenProviders();

builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.AddConsole();
    loggingBuilder.AddDebug();
});

builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

builder.Services.AddSystemWebAdapters();
builder.Services.AddHttpForwarder();

builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddControllersWithViews().AddJsonOptions(opts => opts.JsonSerializerOptions.PropertyNamingPolicy = null);
builder.Services.AddDistributedMemoryCache();
builder.Services.AddMvc(options =>
{
    options.Filters.Add(new BaseResultFilter());
});
builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
}).AddCookie(options =>
{
    options.Cookie.Name = "InterventCookie";
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(builder.Configuration.GetValue<int>("AppSettings:SessionTimeOut"));
    options.LoginPath = "/Home/Index";
    options.LogoutPath = "/Account/LogOut";
    options.AccessDeniedPath = "/Account/NotAuthorized";
    options.SlidingExpiration = true;
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.IncludeErrorDetails = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration.GetValue<string>("AppSettings:JwtValidIssuer"),
        ValidAudience = builder.Configuration.GetValue<string>("AppSettings:JwtValidAudience"),
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("AppSettings:JwtSecret")!)),
        AuthenticationType = "Bearer"
    };
});

Translate.Configure(new HttpContextAccessor());

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(builder.Configuration.GetValue<int>("AppSettings:SessionTimeOut"));
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Account/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

Intervent.Web.DataLayer.Utility.InitMapper();

app.UseRequestLocalization();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();

app.UseSystemWebAdapters();
app.UseSession();
app.MapRazorPages();

app.Use(async (context, next) =>
{
    if (context.Session.IsAvailable && !context.Session.Keys.Contains("SessionStartTime"))
    {
        context.Session.SetString("SessionStartTime", DateTime.Now.ToString());
    }
    await next();
});

app.MapControllerRoute("SamlHome", "Saml/Provider/{id?}", new { controller = "Saml", action = "Provider" });

app.MapAreaControllerRoute("HelpPage_Default", "HelpPage", "Help/{action=Index}/{apiId?}", new { controller = "Help" });

app.MapControllerRoute("ACCLandingPage", "ACC/{id?}", new { controller = "Home", action = "ACC" });

app.MapControllerRoute("ActivateLandingPage", "Activate/{id?}", new { controller = "Home", action = "Activate" });

app.MapControllerRoute("ASPCPage", "ASPC/{id?}", new { controller = "Home", action = "ASPC" });

app.MapControllerRoute("CanRiskPage", "CanRisk/{id?}", new { controller = "Home", action = "CanRisk" });

app.MapControllerRoute("CAPTIVAPage", "CAPTIVA/{id?}", new { controller = "Home", action = "CAPTIVA" });

app.MapControllerRoute("CHIGlenwoodLandingPage", "CHIGlenwood/{id?}", new { controller = "Home", action = "CHIGlenwood" });

app.MapControllerRoute("CHIHixsonLandingPage", "CHIHixson/{id?}", new { controller = "Home", action = "CHIHixson" });

app.MapControllerRoute("CHIMemorialLandingPage", "CHIMemorial/{id?}", new { controller = "Home", action = "CHIMemorial" });

app.MapControllerRoute("CityofPoolerLandingPage", "CityofPooler/{id?}", new { controller = "Home", action = "CityofPooler" });

app.MapControllerRoute("ClientTrialPage", "ClientTrial/{id?}", new { controller = "Home", action = "ClientTrial" });

app.MapControllerRoute("CompassLandingPage", "CompassGroup/{id?}", new { controller = "Home", action = "Compass" });

app.MapControllerRoute("CompassPage", "Compass/{id?}", new { controller = "Home", action = "Compass" });

app.MapControllerRoute("CrothallLandingPage", "Crothall/{id?}", new { controller = "Home", action = "Crothall" });

app.MapControllerRoute("DemoCoachPage", "DemoCoach/{id?}", new { controller = "Home", action = "DemoCoach" });

app.MapControllerRoute("DemoHRAPage", "DemoHRA/{id?}", new { controller = "Home", action = "DemoHRA" });

app.MapControllerRoute("DemoPage", "Demo/{id?}", new { controller = "Home", action = "Demo" });

app.MapControllerRoute("eBenLandingPage", "eBen/{id?}", new { controller = "Home", action = "eBen" });

app.MapControllerRoute("EdlogicsLandingPage", "Edlogics/{id?}", new { controller = "Home", action = "Edlogics" });

app.MapControllerRoute("EmiratesLandingPage", "EmiratesNBD/{id?}", new { controller = "Home", action = "Emirates" });

app.MapControllerRoute("FOLLOWMDLandingPage", "FOLLOWMD/{id?}", new { controller = "Home", action = "FOLLOWMD" });

app.MapControllerRoute("FullCircleLandingPage", "FullCircle/{id?}", new { controller = "Home", action = "FullCircle" });

app.MapControllerRoute("HealthBFWLandingPage", "HealthBFW/{id?}", new { controller = "Home", action = "HealthBFW" });

app.MapControllerRoute("InterventLandingPage", "Intervent/{id?}", new { controller = "Home", action = "Intervent" });

app.MapControllerRoute("IntuityLandingPage", "Intuity/{id?}", new { controller = "Home", action = "Intuity" });

app.MapControllerRoute("JanssenLandingPage", "Janssen/{id?}", new { controller = "Home", action = "Janssen" });

app.MapControllerRoute("JPMCPage", "JPMC/{id?}", new { controller = "Home", action = "JPMC" });

app.MapControllerRoute("LHCPage", "LifeHealthcare/{id?}", new { controller = "Home", action = "LifeHealthcare" });

app.MapControllerRoute("LMCPage", "LMC/{id?}", new { controller = "Home", action = "LMC" });

app.MapControllerRoute("LMCSelfHelpLandingPage", "LMCSelfHelp/{id?}", new { controller = "Home", action = "LMCSelfHelp" });

app.MapControllerRoute("MacPapersLandingPage", "MacPapers/{id?}", new { controller = "Home", action = "MacPapers" });

app.MapControllerRoute("MaxisLandingPage", "MaxisGBNdemo/{id?}", new { controller = "Home", action = "MaxisGBNdemo" });

app.MapControllerRoute("McAllenLandingPage", "McAllen/{id?}", new { controller = "Home", action = "McAllen" });

app.MapControllerRoute("MCITheDoctorsOffice", "MCITheDoctorsOffice/{id?}", new { controller = "Home", action = "MCITheDoctorsOffice" });

app.MapControllerRoute("MetLifeGulfLandingPage", "MetLifeGulf/{id?}", new { controller = "Home", action = "MetLifeGulf" });

app.MapControllerRoute("MetLifeLandingPage", "MetLifeDemo/{id?}", new { controller = "Home", action = "MetLifeDemo" });

app.MapControllerRoute("NewCoPage", "NewCo/{id?}", new { controller = "Home", action = "NewCo" });

app.MapControllerRoute("OptifastLandingPage", "Optifast/{id?}", new { controller = "Home", action = "Optifast" });

app.MapControllerRoute("POGOLandingPage", "POGO/{id?}", new { controller = "Home", action = "POGO" });

app.MapControllerRoute("PoplarLandingPage", "Poplar/{id?}", new { controller = "Home", action = "Poplar" });

app.MapControllerRoute("RetailLandingPage", "tlc/{id?}", new { controller = "Home", action = "tlc" });

app.MapControllerRoute("RiverviewLandingPage", "Riverview/{id?}", new { controller = "Home", action = "Riverview" });

app.MapControllerRoute("ServiceAgreement", "ServiceAgreement/{id?}", new { controller = "Home", action = "ServiceAgreement" });

app.MapControllerRoute("SouthUniversityLandingPage", "SouthUniversity/{id?}", new { controller = "Home", action = "SouthUniversity" });

app.MapControllerRoute("TeamsBPPage", "TeamsBP/{id?}", new { controller = "Home", action = "TeamsBP" });

app.MapControllerRoute("TrainingLandingPage", "Training/{id?}", new { controller = "Home", action = "Training" });

app.MapControllerRoute("GetStepDetails", "Kit/KitStepDetails/{kitId?}/{pageIdentifier?}/{kitsInUserProgramsId?}/{languageCode?}", new { controller = "Kit", action = "KitStepDetails" });

app.MapControllerRoute("GetActivity", "Kit/Activity/{kitId?}/{id?}/{activityId?}", new { controller = "Kit", action = "Activity" });

app.MapControllerRoute("CanRisk1Page", "CanRisk1/{id?}", new { controller = "Home", action = "CanRisk1" });

app.MapDefaultControllerRoute();
app.MapForwarder("/{**catch-all}", app.Configuration["ProxyTo"]).Add(static builder => ((RouteEndpointBuilder)builder).Order = int.MaxValue);

app.MapControllerRoute("Default", "{controller=Home}/{action=Index}/{id}");

app.Run();
