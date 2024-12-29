using System;
using NetDaemon.AppModel;
using Microsoft.Extensions.DependencyInjection;
using NetDaemon.HassModel;
using NetDaemon.HassModel.Entities;
using HomeAssistantGenerated;
using System.Reactive.Concurrency;
using NetDaemon.Extensions.Scheduler;

namespace HomeAssistantApps;
public static class NewMoonNotifierExtensions
{
    ///<summary>Registers all injectable generated types in the serviceCollection</summary>
    public static IServiceCollection AddNewMoonNotifier(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<NewMoonNotifier>();
        return serviceCollection;
    }
}

[NetDaemonApp(Id = "NewMoonNotifier")]
public class NewMoonNotifier
{
    private readonly IHaContext _haContext;
    private readonly Services _services;
    private readonly INetDaemonScheduler _scheduler;

    public NewMoonNotifier(IHaContext haContext, INetDaemonScheduler scheduler)
    {
        _haContext = haContext;
        _services = new Services(haContext);
        _scheduler = scheduler;

        _scheduler.RunEvery(TimeSpan.FromMinutes(2), UpdateNewMoonSensors);
        Scheduler.Default.ScheduleCron("0 17 * * *", SendWhatsAppIfNeeded);
        Scheduler.Default.ScheduleCron("0 08 * * *", NotifyNextNewMoon);
    }
    private void SendWhatsAppIfNeeded()
    {
        try
        {
            var currentDate = DateTime.UtcNow;
            var nextNewMoonInfo = CalculateNextNewMoon(currentDate);

            // Check if hours until next new moon are less than 72
            if (nextNewMoonInfo.HoursUntilNextNewMoon < 72)
            {
                SendWhatsAppMessage($"Reminder: The next New Moon is in {nextNewMoonInfo.HoursUntilNextNewMoon:F2} hours ({nextNewMoonInfo.NextNewMoon:yyyy-MM-dd HH:mm:ss} UTC).");
            }
        }
        catch (Exception ex)
        {
            _services.SystemLog.Write($"Error in SendWhatsAppIfNeeded: {ex.Message}", "error", "NewMoonNotifier");
        }
    }
    private void SendWhatsAppMessage(string message)
    {
        try
        {
            _haContext.CallService(
                domain: "whatsapp",
                service: "send_message",
                data: new
                {
                    clientId = "default",
                    to = "27711304241@s.whatsapp.net",
                    body = new
                    {
                        text = message
                    }
                }
            );
        }
        catch (Exception ex)
        {
            _services.SystemLog.Write($"Error sending WhatsApp message: {ex.Message}", "error", "NewMoonNotifier");
        }
        _services.SystemLog.Write($"WhatsApp message sent: {message}", "info", "NewMoonNotifier");
    }
    private void NotifyNextNewMoon()
    {
        try
        {
            var currentDate = DateTime.UtcNow;
            var nextNewMoonInfo = CalculateNextNewMoon(currentDate);

            var message = $"Current Date: {currentDate:yyyy-MM-dd HH:mm:ss} UTC\n" +
                          $"Next New Moon: {nextNewMoonInfo.NextNewMoon:yyyy-MM-dd HH:mm:ss} UTC\n" +
                          $"Hours Until Next New Moon: {nextNewMoonInfo.HoursUntilNextNewMoon:F2}";

            _services.PersistentNotification.Create(message, "Next New Moon Info");
            _services.SystemLog.Write("Next new moon notification sent successfully.", "info", "NewMoonNotifier");
        }
        catch (Exception ex)
        {
            _services.SystemLog.Write($"Error in NotifyNextNewMoon: {ex.Message}", "error", "NewMoonNotifier");
        }
    }

    private void UpdateNewMoonSensors()
    {
        try
        {
            var currentDate = DateTime.UtcNow;
            var nextNewMoonInfo = CalculateNextNewMoon(currentDate);

            UpdateSensorState("input_number.new_moon_in_days", nextNewMoonInfo.DaysUntilNextNewMoon, "days", "New Moon in Days");
            UpdateSensorState("input_number.new_moon_in_hours", nextNewMoonInfo.HoursUntilNextNewMoon, "hours", "New Moon in Hours");
            UpdateSensorState("input_number.new_moon_in_minutes", nextNewMoonInfo.MinutesUntilNextNewMoon, "minutes", "New Moon in Minutes");
        }
        catch (Exception ex)
        {
            _services.SystemLog.Write($"Error in UpdateNewMoonSensors: {ex.Message}", "error", "NewMoonNotifier");
        }
        _services.SystemLog.Write("New moon sensors updated successfully.", "info", "NewMoonNotifier");
    }

    private void UpdateSensorState(string sensorId, double value, string unit, string friendlyName)
    {
        try
        {
            _haContext.CallService(
                domain: "input_number",
                service: "set_value",
                data: new
                {
                    entity_id = sensorId,
                    value = value
                }
            );
        }
        catch (Exception ex)
        {
            _services.SystemLog.Write($"Error updating sensor {sensorId}: {ex.Message}", "error", "NewMoonNotifier");
        }
    }


    private static NewMoonInfo CalculateNextNewMoon(DateTime currentDate)
    {
        var epoch = new DateTime(2000, 1, 6, 18, 14, 0, DateTimeKind.Utc);
        var synodicMonth = 29.530588861; // Average lunar month in days

        var daysSinceEpoch = (currentDate - epoch).TotalDays;
        var currentPhase = (daysSinceEpoch / synodicMonth) % 1;
        var daysToNextNewMoon = Math.Round((1 - currentPhase) * synodicMonth, 2);

        var nextNewMoon = currentDate.AddDays(daysToNextNewMoon);

        return new NewMoonInfo
        {
            NextNewMoon = nextNewMoon,
            DaysUntilNextNewMoon = daysToNextNewMoon,
            HoursUntilNextNewMoon = Math.Round((nextNewMoon - currentDate).TotalHours, 2),
            MinutesUntilNextNewMoon = Math.Round((nextNewMoon - currentDate).TotalMinutes, 2)
        };
    }

    private class NewMoonInfo
    {
        public DateTime NextNewMoon { get; set; }
        public double DaysUntilNextNewMoon { get; set; }
        public double HoursUntilNextNewMoon { get; set; }
        public double MinutesUntilNextNewMoon { get; set; }
    }
}
