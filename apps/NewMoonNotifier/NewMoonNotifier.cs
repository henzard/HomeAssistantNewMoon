using HomeAssistantGenerated;
using System.Reactive.Concurrency;
using NetDaemon.Extensions.Scheduler;
using Microsoft.Extensions.Logging;

namespace HomeAssistantApps
{
    [NetDaemonApp(Id = "NewMoonNotifier")]
    public class NewMoonNotifier
    {
        private readonly ILogger<NewMoonNotifier> _logger;
        private readonly IHaContext _haContext;
        private readonly Services _services;
        private readonly INetDaemonScheduler _scheduler;

        public NewMoonNotifier(ILogger<NewMoonNotifier> logger, IHaContext haContext, INetDaemonScheduler scheduler)
        {
            
            _logger = logger;
            _logger.LogInformation("NewMoonNotifier initialized.");
            _haContext = haContext;
            _services = new Services(haContext);
            _scheduler = scheduler;
            try
            {

                _scheduler.RunEvery(TimeSpan.FromMinutes(15), UpdateNewMoonSensors);
                Scheduler.Default.ScheduleCron("0 17 * * *", SendWhatsAppIfNeeded);
                Scheduler.Default.ScheduleCron("0 08 * * *", NotifyNextNewMoon);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error in NewMoonNotifier constructor");
                throw;
            }
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
                _logger.LogError(ex, "Error in SendWhatsAppIfNeeded");
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
                _logger.LogError(ex, "Error sending WhatsApp message");
            }
            _logger.LogInformation("WhatsApp message sent: {Message}", message);
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
                _logger.LogInformation("Next new moon notification sent successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in NotifyNextNewMoon: {Message}", ex.Message);
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
                _logger.LogError("Error in UpdateNewMoonSensors: {Message}", ex.Message);
            }
            _logger.LogInformation("New moon sensors updated successfully.");
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
                _logger.LogError("Error updating sensor {sensorId}: {Message}", sensorId, ex.Message);
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
}
