# Звіт з аналізу SOLID принципів (SRP, OCP) в Open-Source проєкті

## 1. Обраний проєкт
- **Назва:** AutoMapper
- **Посилання на GitHub:** [https://github.com/AutoMapper/AutoMapper](https://github.com/AutoMapper/AutoMapper)

## 2. Аналіз SRP (Single Responsibility Principle)

### 2.1. Приклади дотримання SRP

#### Клас: `Profile`
- **Відповідальність:** Групування та ізоляція конфігурації мапінгу для конкретного модуля або функціональної області.
- **Обґрунтування:** Цей клас не виконує мапінг і не валідує його під час виконання. Його єдина причина для зміни — це зміна бізнес-вимог до налаштувань (наприклад, додали нову пару "Source-Destination"). Він діє суто як контейнер для правил, відокремлюючи конфігурацію від виконання.

```csharp
public class UserProfile : Profile
{
    public UserProfile()
    {
        // Єдина відповідальність: визначення правил мапінгу
        CreateMap<User, UserDto>();
        CreateMap<Order, OrderDto>()
            .ForMember(dest => dest.Total, opt => opt.Ignore());
    }
}
```
### 2.1. Приклади дотримання SRP

#### Клас: `UserProfile`

- **Відповідальність:** Визначення правил мапінгу (налаштувань).
- **Обґрунтування:** Клас не виконує логіку, а лише містить конфігурацію.

```csharp
public class UserProfile : Profile
{
    public UserProfile()
    {
        // Єдина відповідальність: визначення правил мапінгу
        CreateMap<User, UserDto>();
        CreateMap<Order, OrderDto>()
            .ForMember(dest => dest.Total, opt => opt.Ignore());
    }
}
```
### 2.2. Приклади порушення SRP

#### Клас: `MapperConfiguration` (або `ResolutionContext` у деяких версіях)

- **Множинні відповідальності:**
    1. Зберігання налаштувань усіх профілів (Configuration Store).
    2. Компіляція планів виконання (Compiler).
    3. Валідація конфігурації (Validator).
    4. Фабрика для створення маперів (Factory).

- **Проблеми:** Це класичний приклад "God Object" (Божественного об'єкта). Клас знає занадто багато про систему. Будь-яка зміна в логіці валідації, компіляції виразів або управлінні пам'яттю вимагає редагування цього файлу. Це ускладнює підтримку та тестування, оскільки клас має високу зв'язність (coupling) з багатьма підсистемами.

```csharp
public class MapperConfiguration : IConfigurationProvider
{
    // Відповідає за ініціалізацію та парсинг налаштувань
    public MapperConfiguration(MapperConfigurationExpression configurationExpression) { ... }

    // Відповідає за створення мапера (Factory)
    public IMapper CreateMapper() { ... }

    // Відповідає за валідацію (Validation logic)
    public void AssertConfigurationIsValid() { ... }

    // Відповідає за компіляцію планів (Compilation logic)
    public void CompileMappings() { ... }
}
```
## 3. Аналіз OCP (Open/Closed Principle)

### 3.1. Приклади дотримання OCP

#### Сценарій/Модуль: Система розширення через `IObjectMapper`

- **Механізм розширення:** Інтерфейс `IObjectMapper` та колекція маперів.
- **Обґрунтування:** Ядро AutoMapper перебирає список об'єктів `IObjectMapper`. Якщо розробнику потрібно додати підтримку мапінгу для специфічного типу (наприклад, `Google.Protobuf` типи), йому не потрібно змінювати код бібліотеки AutoMapper. Достатньо створити новий клас, реалізувати інтерфейс і додати його в конфігурацію. Система "закрита для модифікації" (основний цикл перебору не змінюється), але "відкрита для розширення".

```csharp
// Інтерфейс для розширення (Strategy Pattern)
public interface IObjectMapper
{
    bool IsMatch(TypePair context);
    Expression MapExpression(...);
}

// Новий функціонал додається новим класом, без зміни ядра
public class CustomEnumMapper : IObjectMapper 
{
    public bool IsMatch(TypePair context) => context.SourceType.IsEnum;
    public Expression MapExpression(...) { /* Custom logic */ }
}
```
#### Сценарій/Модуль: `IValueResolver` (Кастомна логіка для полів)

- **Механізм розширення:** Generic-інтерфейс `IValueResolver<TSource, TDest, TDestMember>`.
- **Обґрунтування:** Дозволяє користувачеві вклинитися в процес мапінгу конкретного поля і виконати довільну логіку без зміни основного коду чи механізму мапінгу AutoMapper.

```csharp
// Розширення логіки через створення нового класу
public class CustomResolver : IValueResolver<Source, Dest, int>
{
    public int Resolve(Source source, Dest destination, int destMember, ResolutionContext context)
    {
        return source.Value + 10; // Кастомна логіка
    }
}

// Використання:
// .ForMember(dest => dest.Total, opt => opt.MapFrom<CustomResolver>());
```
### 3.2. Приклади порушення OCP

#### Сценарій/Модуль: Внутрішні утиліти для перевірки типів (Reflection Helpers)

- **Проблема:** У деяких внутрішніх методах для перевірки сумісності типів або визначення, чи є тип колекцією, використовуються жорсткі перевірки через `if-else` або явне перерахування типів.
- **Наслідки:** Якщо у мові C# з'являється нова концепція (наприклад, `Span<T>` або `Record types`), цей код доводиться відкривати та модифікувати, дописуючи нові гілки умов. Це порушує OCP, оскільки система вимагає модифікації існуючого коду для підтримки нових стандартних типів.

```csharp
internal static bool IsCollectionType(Type type)
{
    // Порушення OCP: код закритий для нових типів без модифікації цього методу
    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ICollection<>))
    {
        return true;
    }
    else if (typeof(IEnumerable).IsAssignableFrom(type) && type != typeof(string))
    {
        return true;
    }
    // Для підтримки нового типу колекції тут доведеться дописувати 'else if'
    return false;
}
```
## 4. Загальні висновки

Проаналізувавши бібліотеку **AutoMapper**, можна зробити наступні висновки щодо якості дизайну:

1.  **Архітектурна зрілість:** Проєкт демонструє високий рівень розуміння принципів SOLID. Особливо це стосується **SRP** у Runtime-частині (виконання мапінгу), де кожен конвертер є маленьким, незалежним класом.

2.  **Гнучкість через OCP:** Найсильнішою стороною є дотримання **Open/Closed Principle**. Використання патернів "Strategy" та "Chain of Responsibility" дозволяє бібліотеці залишатися актуальною роками, дозволяючи спільноті писати розширення для нових типів даних без втручання в ядро.

3.  **Виправдані порушення:** Порушення SRP (наявність "God Objects" типу `MapperConfiguration`) є свідомим архітектурним рішенням. Це зроблено для забезпечення зручного API для розробників (Fluent Interface) та централізації складного стану, що спрощує використання бібліотеки, хоч і ускладнює її внутрішню підтримку.

В цілому, **AutoMapper** є чудовим прикладом балансу між академічною чистотою коду (SOLID) та прагматизмом реального світу (зручність використання та продуктивність).