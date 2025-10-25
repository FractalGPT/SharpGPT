# SharpGPT

![Stars](https://img.shields.io/github/stars/FractalGPT/SharpGPT?style=flat-square)
![Forks](https://img.shields.io/github/forks/FractalGPT/SharpGPT?style=flat-square)
![Watchers](https://img.shields.io/github/watchers/FractalGPT/SharpGPT?style=flat-square)
![License](https://img.shields.io/github/license/FractalGPT/SharpGPT?style=flat-square)
![.NET](https://img.shields.io/badge/.NET-Standard%202.0-purple?style=flat-square)

<img src="https://github.com/FractalGPT/SharpGPT/blob/main/IMG/logo.svg?raw=true" width=500 />

## 🚀 **SharpGPT от ООО "АватарМашина"** 🚀

Современная кроссплатформенная библиотека на C# для работы с большими языковыми моделями (LLM) и AI-сервисами от компании [ООО "АватарМашина"](https://fractalgpt.ru/).

---

## ✨ **Основные возможности**

### 🔌 **Универсальный API для LLM**
- 🤖 **OpenAI** (ChatGPT, GPT-4, GPT-3.5)
- 🧠 **Anthropic** (Claude, DeepSeek)
- 🌐 **Google AI Studio** (Gemini)
- 🔀 **OpenRouter** - доступ к десяткам моделей
- 💡 **Perplexity AI**
- 🏠 **vLLM** - локальное развертывание моделей
- 🖥️ **Локальные серверы** - работа без интернета

### 🛠️ **Расширенные возможности**
- 📝 **Embeddings** - векторизация текстов
- 🔍 **Reranking** - переранжирование результатов поиска
- 🎨 **Image Generation** - генерация изображений
- 💬 **Streaming** - потоковая передача ответов
- 🔄 **Few-Shot Learning** - обучение на примерах
- 👤 **Persona Chat** - персонализированные диалоги

### 🎯 **Решаемые задачи**
- 📚 Суммаризация текстов
- 💬 Диалоговые системы
- 📋 Вопросно-ответные системы (QA)
- 🧮 Решение математических задач
- 📄 Работа с документами
- 🔍 Семантический поиск
- 🎭 Классификация текстов
- ✍️ Генерация контента

---

## 🏗️ **Архитектура проекта**

```
SharpGPT/
├── Core/                    # Базовые абстракции и модели
│   ├── Abstractions/        # Интерфейсы
│   ├── Models/              # Модели данных
│   │   ├── Common/          # Общие модели
│   │   └── Providers/       # Специфичные для провайдеров
│   └── Exceptions/          # Исключения
│
├── Clients/                 # API клиенты
│   ├── OpenAI/             # ChatGPT
│   ├── Anthropic/          # Claude, DeepSeek
│   ├── Google/             # Gemini
│   ├── VLLM/               # vLLM
│   └── LocalServer/        # Локальные серверы
│
├── Services/                # Бизнес-логика
│   ├── LLM/                # Работа с языковыми моделями
│   ├── Embeddings/         # Векторизация
│   ├── Reranking/          # Переранжирование
│   └── Prompts/            # Управление промптами
│
├── Infrastructure/          # Инфраструктурный код
│   └── Http/               # HTTP клиенты
│
└── Utilities/              # Вспомогательные утилиты
    └── Extensions/         # Расширения
```

---

## 🚀 **Быстрый старт**

### Установка

```bash
# Клонирование репозитория
git clone https://github.com/FractalGPT/SharpGPT.git

# Переход в директорию проекта
cd SharpGPT/src
```
---

## 🌟 **Почему C#?**

### 1️⃣ **Производительность**
🚀 C# — компилируемый язык с высокой производительностью, что критично для работы с большими объемами данных и моделями.

### 2️⃣ **Интеграция с .NET экосистемой**
🔗 Бесшовная интеграция с Unity, ASP.NET, WPF, Xamarin, MAUI и другими фреймворками .NET.

### 3️⃣ **Кроссплатформенность**
🖥️ Работает на Windows, Linux, macOS благодаря .NET Standard 2.0.

### 4️⃣ **Стабильность и поддержка**
🛡️ Многолетняя поддержка от Microsoft и активное сообщество разработчиков.

### 5️⃣ **Безопасность**
🔒 Строгая типизация, контроль доступа и современные средства обеспечения безопасности кода.

### 6️⃣ **Современные возможности**
✨ Async/await, LINQ, pattern matching, records и другие современные языковые конструкции.

---

## 🛣️ **Дорожная карта**

### 🌐 **API и интеграции**

| Функционал | Статус |
|-----------|--------|
| OpenAI (ChatGPT, GPT-4) | ✅ |
| DeepSeek | ✅ |
| Google AI Studio (Gemini) | ✅ |
| OpenRouter | ✅ |
| Perplexity AI | ✅ |
| vLLM | ✅ |
| Локальный сервер | ✅ |
| Infinity Embeddings | ✅ |
| Infinity Reranking | ✅ |
| VLLM Reranking | ✅ |
| Image Generation | ✅ |
| Streaming | ✅ |
| Function Calling | 🔄 |
| Vision API | 🔄 |
| Audio API | ❌ |

### 📋 **Задачи и возможности**

#### ✅ Реализовано
- Суммаризация текстов
- Диалоговые системы
- Вопрос-ответ по тексту
- Генерация контента (описания, письма, код)
- Персонализированный чат
- Решение математических задач
- Работа с документами
- Few-Shot обучение
- Векторизация и поиск
- Переранжирование результатов

#### 🔄 В разработке
- Автоматическое создание обзоров
- Проверка галлюцинаций
- Модуль логического вывода
- Vision API интеграция

#### ❌ Планируется
- Работа с поисковыми системами
- Быстрообучаемые классификаторы
- Прогнозирование временных рядов
- Синтез речи (TTS)
- Управление ПК через AI

---

## 🤝 **Вклад в проект**

Мы приветствуем вклад сообщества! 

### Как помочь проекту:
1. 🌟 Поставьте звезду проекту
2. 🔀 Сделайте Fork
3. 🔧 Создайте feature branch (`git checkout -b feature/AmazingFeature`)
4. 💾 Закоммитьте изменения (`git commit -m 'Add some AmazingFeature'`)
5. 📤 Сделайте Push (`git push origin feature/AmazingFeature`)
6. 🎉 Откройте Pull Request

### Правила контрибуции:
- Следуйте архитектуре проекта
- Используйте XML-документацию для публичных API
- Пишите unit-тесты для новой функциональности
- Придерживайтесь стиля кода проекта

---

## 📦 **Методы распространения**

| Метод | Статус |
|-------|--------|
| Исходный код (GitHub) | ✅ |
| NuGet пакет | 🔄 В разработке |
| Скомпилированное приложение | 📅 Планируется |

---

## 🔗 **Связанные проекты**

- [FractalGPT](https://fractalagents.ai/) - Платформа для работы с AI
- [SimpleLLMServer](https://github.com/FractalGPT/SimpleLLMServer) - Локальный сервер для LLM
- [AI Framework](https://github.com/AIFramework/AIFrameworkOpen) - Фреймворк для машинного обучения

---

## 📧 **Контакты**

- 🌐 Сайт: [fractalgpt.ru](https://fractalagents.ai/)
- 💼 GitHub: [FractalGPT](https://github.com/FractalGPT)
- 📧 Email: support@fractalgpt.ru

---

## 📄 **Лицензия**

Этот проект лицензирован под [Apache License 2.0](./LICENSE)

```
Copyright 2024 ООО "АватарМашина"

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
```

---

## **Благодарности**

Спасибо всем контрибьюторам и пользователям библиотеки за поддержку проекта!