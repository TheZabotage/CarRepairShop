using Microsoft.Extensions.Logging;
using CarRepairShop.ViewModels;
using CarRepairShop.Pages;
using CarRepairShop.Services;
using CarRepairShop.Converters;

namespace CarRepairShop
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Register services
            builder.Services.AddSingleton<DatabaseService>();

            // Register converters
            builder.Services.AddSingleton<StringNotNullOrEmptyBoolConverter>();
            builder.Services.AddSingleton<ObjectNotNullConverter>();
            builder.Services.AddSingleton<MultiplyByConverter>();

            // Register view models
            builder.Services.AddSingleton<BookingViewModel>();
            builder.Services.AddSingleton<CalendarViewModel>();
            builder.Services.AddTransient<InvoiceViewModel>();

            // Register pages
            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddSingleton<BookingPage>();
            builder.Services.AddSingleton<CalendarPage>();
            builder.Services.AddTransient<InvoicePage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}