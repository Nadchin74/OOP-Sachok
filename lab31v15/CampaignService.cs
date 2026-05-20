using System;

namespace lab31v15
{
    // Модель рекламної кампанії
    public class Campaign
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }

    // Інтерфейс для роботи з базою даних кампаній
    public interface ICampaignRepository
    {
        Campaign GetCampaign(int id);
        void SaveCampaign(Campaign campaign);
    }

    // Інтерфейс для аналітики
    public interface IAnalyticsService
    {
        void LogEvent(string eventName, int campaignId);
        bool IsCampaignProfitable(int id);
    }

    // Сервіс, який містить бізнес-логіку
    public class CampaignService
    {
        private readonly ICampaignRepository _repository;
        private readonly IAnalyticsService _analytics;

        // Dependency Injection через конструктор
        public CampaignService(ICampaignRepository repository, IAnalyticsService analytics)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _analytics = analytics ?? throw new ArgumentNullException(nameof(analytics));
        }

        // Запуск кампанії
        public bool StartCampaign(int id)
        {
            var campaign = _repository.GetCampaign(id);
            if (campaign == null)
                throw new ArgumentException("Кампанію не знайдено.");

            if (campaign.IsActive)
                return false; // Вже запущена

            campaign.IsActive = true;
            _repository.SaveCampaign(campaign);
            _analytics.LogEvent("CampaignStarted", id);
            
            return true;
        }

        // Зупинка кампанії
        public bool StopCampaign(int id)
        {
            var campaign = _repository.GetCampaign(id);
            if (campaign == null)
                throw new ArgumentException("Кампанію не знайдено.");

            if (!campaign.IsActive)
                return false; // Вже зупинена

            campaign.IsActive = false;
            _repository.SaveCampaign(campaign);
            _analytics.LogEvent("CampaignStopped", id);
            
            return true;
        }

        // Аналіз і зупинка, якщо збиткова
        public bool AnalyzeAndStopIfUnprofitable(int id)
        {
            bool isProfitable = _analytics.IsCampaignProfitable(id);
            if (!isProfitable)
            {
                return StopCampaign(id);
            }
            return false;
        }
    }
}