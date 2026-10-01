<p align="center">
  <img src="WorkoutTracker.Client/wwwroot/images/logo.svg" alt="Workout Tracker logo" width="180">
</p>

# Workout Tracker

![Platform](https://img.shields.io/badge/platform-Web-green)
![Language](https://img.shields.io/badge/language-C%23-blue)
![Frontend](https://img.shields.io/badge/frontend-Blazor%20WebAssembly-orange)
![Backend](https://img.shields.io/badge/backend-ASP.NET%20Core-512BD4)
![Database](https://img.shields.io/badge/database-PostgreSQL-316192)
![Status](https://img.shields.io/badge/status-in%20development-yellow)

Workout Tracker - веб-трекер для тренировок. Пользователь может создать тренировку, выбрать или добавить упражнения, заполнить подходы, вес и количество повторений, а затем посмотреть историю и статистику тренировок.

Основной сценарий использования приложения - ведение дневника тренировок, с помощью которого будет удобней прогрессировать, увеличивая рабочие веса и повторения.

Безусловно, дневник тренировок можно вести в заметках на телефоне или на бумаге в блокноте, но мое приложение собирает статистику по тренировкам, предоставляет тренировки в красивом структурированном виде, а также запоминает уже добавленные упражнения.
# Contents

- [About](#about)
- [Шаги для установки](#шаги-для-установки)
- [Environment](#environment)
- [Tech Stack](#tech-stack)
- [Domain Model](#domain-model)
- [API](#api)
- [Deploy](#deploy)
- [Author](#author)

# About

Что можно делать в Workout Tracker:

- регистрироваться и входить в аккаунт
- создавать тренировку на выбранную дату
- выбирать упражнение из личного списка упражнений
- создавать новое упражнение, если его ещё нет
- добавлять подходы, вес и повторения
- смотреть тренировки за сегодня
- открывать историю тренировок
- редактировать тренировку
- удалять тренировки и упражнения из тренировки
- смотреть статистику: количество тренировок, общий вес и streak по тренировкам

# Шаги для установки

1. Склонируйте репозиторий.

2. Создайте `.env` на основе `.env.example`:

   ```bash
   cp .env.example .env
   ```

3. Заполните переменные окружения для ASP.NET Core, PostgreSQL и client origin.

4. Запустите инфраструктуру через Docker Compose:

   ```bash
   docker compose up -d --build
   ```

5. Примените миграции базы данных:

   ```bash
   dotnet ef database update --project WorkoutTracker.Api/WorkoutTracker.Api.csproj
   ```

Для локального запуска без Docker можно запустить API и клиент отдельно:

```bash
dotnet run --project WorkoutTracker.Api
dotnet run --project WorkoutTracker.Client
```

# Environment

Требующиеся переменные окружения можно посмотреть в файле `.env.example`.

# Tech Stack

### Backend

- C#
- ASP.NET Core Minimal API
- ASP.NET Core Identity
- Entity Framework Core
- PostgreSQL

Backend находится в `WorkoutTracker.Api`. API построен на Minimal API endpoints, бизнес-логика вынесена в сервисы.

### Infrastructure

- Docker
- Docker Compose
- Nginx
- HTTPS через nginx reverse proxy

### Frontend

- Blazor WebAssembly 💀
- Razor Components
- CSS isolation
- Cookie authentication state provider

Пользовательский интерфейс находится в `WorkoutTracker.Client`. Основные страницы:

- `Home`
- `Login`
- `Register`
- `CreateWorkout`
- `WorkoutHistory`
- `WorkoutDetails`

Также для удобства отдельные части страниц вынесены в компоненты. Примеры компонент:
- `AppHeader`
- `ExerciseCard`
- `SetsTable` и другие

# Domain Model

Основные сущности приложения:

- `ApplicationUser` - пользователь приложения, расширение Identity user с хранением имени.
- `Workout` - тренировка пользователя на конкретную дату.
- `Exercise` - упражнение пользователя.
- `WorkoutExercise` - связь тренировки и упражнения.
- `Set` - подход внутри упражнения: номер подхода, вес и количество повторений.

Тренировки и упражнения привязаны к пользователю через `UserId`, чтобы каждый пользователь видел только свои данные.

# API

Основные группы ручек:

- `POST /api/register` - регистрация пользователя с именем.
- `GET /api/profile` - получение данных текущего пользователя.
- `POST /login` - вход через Identity cookies.
- `POST /logout` - выход из аккаунта.
- `GET /api/workouts` - список тренировок пользователя.
- `POST /api/workouts` - создание тренировки.
- `GET /api/workouts/{workoutId}` - получение тренировки.
- `DELETE /api/workouts/{workoutId}` - удаление тренировки.
- `GET /api/exercises` - список упражнений пользователя.
- `POST /api/exercises` - создание упражнения.
- `GET /api/workouts/{workoutId}/exercises` - упражнения внутри тренировки.
- `POST /api/workouts/{workoutId}/exercises` - добавление упражнения в тренировку.
- `GET /api/workouts/{workoutId}/exercises/{workoutExerciseId}/sets` - подходы упражнения.
- `POST /api/workouts/{workoutId}/exercises/{workoutExerciseId}/sets` - добавление подхода.

# Deploy

В production приложение запускается в Docker Compose:

- `postgres` — база данных PostgreSQL
- `api` — ASP.NET Core API
- `client` — Blazor WebAssembly, раздаётся через nginx

Nginx отдаёт статический frontend и проксирует API-запросы в backend-контейнер.

# Author

- Казанцева Илона — fullstack-разработка, UI, backend, авторизация, база данных, Docker, nginx, HTTPS и деплой.

