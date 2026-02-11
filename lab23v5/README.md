# Лабораторна робота №23: ISP, DIP та Dependency Injection

**Тема:** Рефакторинг системи UserAccountManager з використанням принципів SOLID.  
**Мета:** Усунути жорстку зв'язаність компонентів та розділити інтерфейси.

## 1. Аналіз порушень (Code Smell)

У початковій версії класу `BadUserAccountManager` було виявлено наступні проблеми:

### Порушення DIP (Dependency Inversion Principle)
Клас безпосередньо створював екземпляри залежностей у конструкторі:
```csharp
// BAD: High-level module creates low-level modules
_db = new SqlDatabase();
_email = new SmtpClientService();
```
**Наслідок:** Неможливо протестувати клас ізольовано (Unit Testing) або замінити базу даних без зміни коду самого менеджера.

### Порушення ISP (Interface Segregation Principle)
Хоча у "поганому" прикладі явних інтерфейсів не було, сам клас `BadUserAccountManager` виступав як моноліт, що знав про реалізацію і SMS, і Email, і БД. У випадку розширення системи, клієнти цього класу залежали б від методів, які їм не потрібні (наприклад, методи відновлення пароля через SMS, коли потрібен лише Email).

## 2. Виконаний рефакторинг

### Крок 1: Застосування ISP
Ми виділили вузькоспеціалізовані інтерфейси:
* `IRepository` — для роботи з даними.
* `IEmailSender` та `ISmsSender` (успадковані від `INotificationService`) — для розділення каналів зв'язку.

### Крок 2: Застосування DIP та DI
Ми змінили `BetterUserAccountManager` так, щоб він залежав від абстракцій (інтерфейсів), а не конкретних класів. Реалізацію передаємо через конструктор:

```csharp
// GOOD: Dependencies are injected
public BetterUserAccountManager(IRepository repo, IEmailSender email, ISmsSender sms)
{
    _repository = repo;
    _emailSender = email;
    _smsSender = sms;
}
```
## 3. Висновки
Впровадження **Dependency Injection** дозволило:

1. **Зменшити зв'язаність (Decoupling):** Клас менеджера більше не знає, яка саме база даних використовується (SQL чи MongoDB).
2. **Покращити тестування:** Тепер ми можемо передати фейкові (Mock) об'єкти замість реальної відправки SMS.
3. **Гнучкість:** Ми можемо додавати нові типи сповіщень, не змінюючи логіку бізнес-класу.