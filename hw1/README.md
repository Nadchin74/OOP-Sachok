# Анти-патерн "God Object" та принцип єдиної відповідальності (SRP)

## 1. Характеристики анти-патерну "God Object"

**God Object** (Божественний об'єкт) — це анти-патерн об'єктно-орієнтованого програмування, який описує об'єкт, що бере на себе занадто багато обов'язків. Замість того щоб делегувати завдання іншим об'єктам, він виконує все самостійно, перетворюючись на величезний моноліт.

**Основні характеристики:**
* **Надмірна функціональність:** Клас містить бізнес-логіку, доступ до даних, валідацію, відображення та логування одночасно.
* **Величезний розмір:** Такі класи часто мають тисячі рядків коду та десятки методів.
* **Тісна зв'язність (High Coupling):** Об'єкт залежить від багатьох інших компонентів системи, тому будь-яка зміна в проекті може зламати цей "Божественний об'єкт".
* **Складність тестування:** Через велику кількість станів та залежностей написати Unit-тести для такого класу майже неможливо.

---

## 2. Приклад порушення SRP

**Single Responsibility Principle (SRP)** стверджує: "Клас повинен мати лише одну причину для змін".

Розглянемо клас `UserManager`, який є прикладом "God Object" у мініатюрі. Він порушує SRP, оскільки виконує непов'язані функції.

### Код із порушенням (Bad Practice)

```python
class UserManager:
    def register_user(self, username, email, password):
        # 1. Валідація даних (Бізнес-правило)
        if "@" not in email:
            raise ValueError("Invalid email")
            
        # 2. Збереження в базу даних (Інфраструктура)
        print(f"Connecting to DB...")
        print(f"INSERT INTO users VALUES ({username}, {email}, {password})")
        
        # 3. Відправка Email (Сповіщення)
        print(f"Sending welcome email to {email}...")
        
        # 4. Логування (Діагностика)
        with open("app.log", "a") as f:
            f.write(f"User {username} registered successfully.\n")
### Чому це погано?

Цей клас має **4 причини для змін**:

1.  Зміна логіки валідації.
2.  Зміна типу бази даних.
3.  Зміна провайдера пошти.
4.  Зміна формату логів.

---

## 3. Рефакторинг (Дотримання SRP)

Щоб виправити ситуацію, ми розділимо великий клас на декілька маленьких, кожен з яких відповідатиме за свою вузьку задачу.

Ми створимо:
* `UserRepository` — для роботи з БД.
* `EmailService` — для пошти.
* `Logger` — для логів.
* `UserService` — для керування процесом.

### Рефакторинг (Best Practice)

```python
# Відповідальність: Робота з даними
class UserRepository:
    def save(self, user):
        print(f"Saving user {user['name']} to database...")

# Відповідальність: Сповіщення
class EmailService:
    def send_welcome_email(self, email):
        print(f"Sending welcome email to {email}...")

# Відповідальність: Логування
class Logger:
    def log(self, message):
        print(f"LOG: {message}")

# Відповідальність: Бізнес-логіка реєстрації (Фасад)
class UserService:
    def __init__(self, repository, email_service, logger):
        self.repository = repository
        self.email_service = email_service
        self.logger = logger

    def register_user(self, username, email, password):
        # Валідація залишається частиною бізнес-логіки
        if "@" not in email:
            raise ValueError("Invalid email")
        
        user = {"name": username, "email": email, "password": password}
        
        # Делегування завдань спеціалізованим класам
        self.repository.save(user)
        self.email_service.send_welcome_email(email)
        self.logger.log(f"User {username} registered.")

# Приклад використання
if __name__ == "__main__":
    service = UserService(UserRepository(), EmailService(), Logger())
    service.register_user("Oleg", "oleg@test.com", "password123")
#Висновок: Тепер кожен клас відповідає лише за свою частину роботи. Код став чистим, гнучким і легким для тестування.