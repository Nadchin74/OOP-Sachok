using System;
using Moq;
using Xunit;
using lab31v15;

namespace lab31v15.Tests
{
    public class CampaignServiceTests
    {
        private readonly Mock<ICampaignRepository> _mockRepo;
        private readonly Mock<IAnalyticsService> _mockAnalytics;
        private readonly CampaignService _service;

        public CampaignServiceTests()
        {
            // Ініціалізація моків перед кожним тестом
            _mockRepo = new Mock<ICampaignRepository>();
            _mockAnalytics = new Mock<IAnalyticsService>();
            
            _service = new CampaignService(_mockRepo.Object, _mockAnalytics.Object);
        }

        // Тест 1: Перевірка Dependency Injection (null репозиторій)
        [Fact]
        public void Constructor_NullRepository_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new CampaignService(null, _mockAnalytics.Object));
        }

        // Тест 2: Перевірка Dependency Injection (null аналітика)
        [Fact]
        public void Constructor_NullAnalytics_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new CampaignService(_mockRepo.Object, null));
        }

        // Тест 3: StartCampaign викидає виняток, якщо кампанію не знайдено
        [Fact]
        public void StartCampaign_CampaignNotFound_ThrowsArgumentException()
        {
            // Setup: GetCampaign повертає null
            _mockRepo.Setup(r => r.GetCampaign(1)).Returns((Campaign)null);

            Assert.Throws<ArgumentException>(() => _service.StartCampaign(1));
        }

        // Тест 4: StartCampaign повертає false, якщо вже активна
        [Fact]
        public void StartCampaign_AlreadyActive_ReturnsFalse()
        {
            var activeCampaign = new Campaign { Id = 1, IsActive = true };
            _mockRepo.Setup(r => r.GetCampaign(1)).Returns(activeCampaign);

            var result = _service.StartCampaign(1);

            Assert.False(result);
            // Verify: перевіряємо, що збереження не викликалося
            _mockRepo.Verify(r => r.SaveCampaign(It.IsAny<Campaign>()), Times.Never);
        }

        // Тест 5: StartCampaign успішно запускає кампанію
        [Fact]
        public void StartCampaign_Inactive_ActivatesAndLogsEvent()
        {
            var inactiveCampaign = new Campaign { Id = 1, IsActive = false };
            _mockRepo.Setup(r => r.GetCampaign(1)).Returns(inactiveCampaign);

            var result = _service.StartCampaign(1);

            Assert.True(result);
            Assert.True(inactiveCampaign.IsActive);
            
            // Verify: перевіряємо, що метод збереження і логування були викликані рівно 1 раз
            _mockRepo.Verify(r => r.SaveCampaign(inactiveCampaign), Times.Once);
            _mockAnalytics.Verify(a => a.LogEvent("CampaignStarted", 1), Times.Once);
        }

        // Тест 6: StopCampaign повертає false, якщо вже зупинена
        [Fact]
        public void StopCampaign_AlreadyInactive_ReturnsFalse()
        {
            var inactiveCampaign = new Campaign { Id = 2, IsActive = false };
            _mockRepo.Setup(r => r.GetCampaign(2)).Returns(inactiveCampaign);

            var result = _service.StopCampaign(2);

            Assert.False(result);
            _mockRepo.Verify(r => r.SaveCampaign(It.IsAny<Campaign>()), Times.Never);
        }

        // Тест 7: StopCampaign успішно зупиняє кампанію
        [Fact]
        public void StopCampaign_Active_DeactivatesAndLogsEvent()
        {
            var activeCampaign = new Campaign { Id = 2, IsActive = true };
            _mockRepo.Setup(r => r.GetCampaign(2)).Returns(activeCampaign);

            var result = _service.StopCampaign(2);

            Assert.True(result);
            Assert.False(activeCampaign.IsActive);
            
            _mockRepo.Verify(r => r.SaveCampaign(activeCampaign), Times.Once);
            _mockAnalytics.Verify(a => a.LogEvent("CampaignStopped", 2), Times.Once);
        }

        // Тест 8: AnalyzeAndStopIfUnprofitable зупиняє збиткову кампанію
        [Fact]
        public void AnalyzeAndStopIfUnprofitable_NotProfitable_StopsCampaign()
        {
            var activeCampaign = new Campaign { Id = 3, IsActive = true };
            _mockRepo.Setup(r => r.GetCampaign(3)).Returns(activeCampaign);
            
            // Setup: Імітуємо, що кампанія збиткова
            _mockAnalytics.Setup(a => a.IsCampaignProfitable(3)).Returns(false);

            var result = _service.AnalyzeAndStopIfUnprofitable(3);

            Assert.True(result); // Успішно зупинена
            Assert.False(activeCampaign.IsActive);
            _mockAnalytics.Verify(a => a.LogEvent("CampaignStopped", 3), Times.Once);
        }

        // Тест 9: AnalyzeAndStopIfUnprofitable не зупиняє прибуткову кампанію
        [Fact]
        public void AnalyzeAndStopIfUnprofitable_Profitable_DoesNothing()
        {
            // Setup: Імітуємо, що кампанія прибуткова
            _mockAnalytics.Setup(a => a.IsCampaignProfitable(4)).Returns(true);

            var result = _service.AnalyzeAndStopIfUnprofitable(4);

            Assert.False(result);
            // Verify: перевіряємо, що спроб зупинити (і зберегти) не було
            _mockRepo.Verify(r => r.GetCampaign(It.IsAny<int>()), Times.Never);
            _mockRepo.Verify(r => r.SaveCampaign(It.IsAny<Campaign>()), Times.Never);
        }
    }
}