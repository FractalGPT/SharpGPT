using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FractalGPT.SharpGPTLib.Clients.OpenRouter;
using FractalGPT.SharpGPTLib.Core.Models.Common.Messages;
using FractalGPT.SharpGPTLib.Core.Models.Common.Requests;
using FractalGPT.SharpGPTLib.Infrastructure.Http;

namespace IdleTimeoutConsoleTests;

/// <summary>
/// Консольное приложение для тестирования Idle Timeout с OpenRouter API
/// 
/// ВАЖНО: SendWithContextAsync ВСЕГДА использует streaming внутри!
/// Это позволяет рано обнаруживать зависшие запросы через idle timeout.
/// </summary>
class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("=".PadRight(80, '='));
        Console.WriteLine("ТЕСТИРОВАНИЕ IDLE TIMEOUT С OPENROUTER API");
        Console.WriteLine("=".PadRight(80, '='));
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("ВАЖНО: SendWithContextAsync ВСЕГДА использует streaming ВНУТРИ!");
        Console.WriteLine("       Это позволяет рано обнаруживать зависшие запросы.");
        Console.WriteLine("       Если между токенами нет ответа > N секунд → TimeoutException");
        Console.ResetColor();
        Console.WriteLine();

        // Получаем API ключ
        string? apiKey = GetApiKey(args);
        if (string.IsNullOrEmpty(apiKey))
        {
            ShowApiKeyHelp();
            return;
        }
        
        Console.WriteLine();

        // Запуск тестов
        var testsRun = 0;
        var testsPassed = 0;
        var testsFailed = 0;

        // Тест 1: Обычный вызов с idle timeout 30 секунд
        testsRun++;
        Console.WriteLine($"[ТЕСТ {testsRun}] Обычный вызов SendWithContextAsync (внутри streaming) с idle timeout 30 сек");
        Console.WriteLine("-".PadRight(80, '-'));
        if (await Test1_NormalCallWithIdleTimeout(apiKey))
            testsPassed++;
        else
            testsFailed++;
        Console.WriteLine();

        // Тест 2: Короткий timeout (10 сек) - проверим что быстрый ответ успевает
        testsRun++;
        Console.WriteLine($"[ТЕСТ {testsRun}] Вызов с КОРОТКИМ idle timeout (10 сек)");
        Console.WriteLine("-".PadRight(80, '-'));
        if (await Test2_ShortIdleTimeout(apiKey))
            testsPassed++;
        else
            testsFailed++;
        Console.WriteLine();

        // Тест 3: Длинный ответ с idle timeout 30 сек
        testsRun++;
        Console.WriteLine($"[ТЕСТ {testsRun}] Длинный ответ (реферат) с idle timeout 30 сек");
        Console.WriteLine("-".PadRight(80, '-'));
        if (await Test3_LongResponseWithIdleTimeout(apiKey))
            testsPassed++;
        else
            testsFailed++;
        Console.WriteLine();

        // Тест 4: Отключенный idle timeout
        testsRun++;
        Console.WriteLine($"[ТЕСТ {testsRun}] Запрос с ОТКЛЮЧЕННЫМ idle timeout");
        Console.WriteLine("-".PadRight(80, '-'));
        if (await Test4_DisabledIdleTimeout(apiKey))
            testsPassed++;
        else
            testsFailed++;
        Console.WriteLine();

        // Итоги
        PrintSummary(testsRun, testsPassed, testsFailed);
        
        Console.WriteLine();
        Console.WriteLine("Нажмите Enter для выхода...");
        Console.ReadLine();
    }

    #region API Key Management

    static string? GetApiKey(string[] args)
    {
        // 1. Проверяем аргументы командной строки
        if (args.Length > 0 && !string.IsNullOrWhiteSpace(args[0]))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("✅ API ключ получен из аргументов командной строки");
            Console.ResetColor();
            return args[0];
        }
        
        // 2. Проверяем переменную окружения
        var envKey = Environment.GetEnvironmentVariable("OPENROUTER_API_KEY");
        if (!string.IsNullOrEmpty(envKey))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("✅ API ключ найден в переменных окружения");
            Console.ResetColor();
            return envKey;
        }
        
        // 3. Запрашиваем у пользователя
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("⚠️  API ключ OpenRouter не найден");
        Console.ResetColor();
        Console.WriteLine();
        Console.WriteLine("Введите API ключ OpenRouter (или нажмите Enter для выхода):");
        Console.WriteLine("Совет: Можно вставить ключ из буфера обмена (Ctrl+V или правая кнопка мыши)");
        Console.Write("> ");
        
        var key = Console.ReadLine();
        
        if (string.IsNullOrWhiteSpace(key))
        {
            return null;
        }
        
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("✅ API ключ получен");
        Console.ResetColor();
        return key;
    }

    static void ShowApiKeyHelp()
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine();
        Console.WriteLine("❌ API ключ не введен. Выход.");
        Console.ResetColor();
        Console.WriteLine();
        Console.WriteLine("Вы можете запустить программу одним из способов:");
        Console.WriteLine();
        Console.WriteLine("  1. С аргументом:");
        Console.WriteLine("     dotnet run -- ваш_api_ключ");
        Console.WriteLine();
        Console.WriteLine("  2. С переменной окружения:");
        Console.WriteLine("     PowerShell: $env:OPENROUTER_API_KEY=\"ваш_ключ\"");
        Console.WriteLine("     CMD:        set OPENROUTER_API_KEY=ваш_ключ");
        Console.WriteLine();
        Console.WriteLine("  3. Ввести вручную при запросе (как сейчас)");
        Console.WriteLine();
    }

    #endregion

    #region Tests

    static async Task<bool> Test1_NormalCallWithIdleTimeout(string apiKey)
    {
        try
        {
            var api = new OpenRouterModelApi(apiKey, "google/gemini-2.5-flash");
            
            // Устанавливаем idle timeout - он будет работать автоматически!
            api.IdleTimeoutSettings = IdleTimeoutSettings.FromTimeSpan(TimeSpan.FromMilliseconds(100));

            Console.WriteLine("Модель: google/gemini-flash-2.5");
            Console.WriteLine("Idle timeout: 30 секунд");
            Console.WriteLine("Запрос: Напиши короткую историю про кота в 2-3 предложениях");
            Console.WriteLine();
            
            var messages = new[]
            {
                new LLMMessage("user", "Напиши короткую историю про кота в 4-6 предложениях")
            };

            var settings = new GenerateSettings(
                temperature: 0.7,
                maxTokens: 150
                // streamId НЕ указываем - метод сам включит streaming внутри!
            );

            Console.Write("Отправка запроса (внутри будет использован streaming)... ");
            var startTime = DateTime.Now;
            var response = await api.SendWithContextAsync(messages, settings, CancellationToken.None);
            var elapsed = (DateTime.Now - startTime).TotalSeconds;
            
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"✓ Ответ получен за {elapsed:F1} секунд");
            Console.ResetColor();
            Console.WriteLine();
            
            var content = response.Choices.First().Message.Content;
            Console.WriteLine($"Ответ: {content}");
            Console.WriteLine();
            
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("✅ ТЕСТ ПРОЙДЕН - Idle timeout не сработал (токены приходили вовремя)");
            Console.ResetColor();
            return true;
        }
        catch (TimeoutException ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"❌ ТЕСТ НЕ ПРОЙДЕН - Idle Timeout сработал: {ex.Message}");
            Console.WriteLine("   Это значит что между токенами прошло > 30 секунд (запрос завис)");
            Console.ResetColor();
            return false;
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"❌ ТЕСТ НЕ ПРОЙДЕН: {ex.Message}");
            Console.WriteLine($"   Тип: {ex.GetType().Name}");
            if (ex.InnerException != null)
                Console.WriteLine($"   Inner: {ex.InnerException.Message}");
            Console.ResetColor();
            return false;
        }
    }

    static async Task<bool> Test2_ShortIdleTimeout(string apiKey)
    {
        try
        {
            var api = new OpenRouterModelApi(apiKey, "google/gemini-2.5-flash");
            
            // Короткий timeout - но ответ должен успеть прийти
            api.IdleTimeoutSettings = IdleTimeoutSettings.FromSeconds(10);

            Console.WriteLine("Модель: google/gemini-2.5-flash");
            Console.WriteLine("Idle timeout: 10 секунд (КОРОТКИЙ)");
            Console.WriteLine("Запрос: Привет!");
            Console.WriteLine();

            var messages = new[]
            {
                new LLMMessage("user", "Привет!")
            };

            var settings = new GenerateSettings(
                temperature: 0.7,
                maxTokens: 20
            );

            Console.Write("Отправка запроса... ");
            var response = await api.SendWithContextAsync(messages, settings, CancellationToken.None);
            
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("✓ Ответ получен");
            Console.ResetColor();
            Console.WriteLine();
            
            var content = response.Choices.First().Message.Content;
            Console.WriteLine($"Ответ: {content}");
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("✅ ТЕСТ ПРОЙДЕН - Быстрый ответ успел прийти за 10 секунд");
            Console.ResetColor();
            return true;
        }
        catch (TimeoutException ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"❌ ТЕСТ НЕ ПРОЙДЕН - Idle Timeout: {ex.Message}");
            Console.WriteLine("   Ответ не успел прийти за 10 секунд");
            Console.ResetColor();
            return false;
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"❌ ТЕСТ НЕ ПРОЙДЕН: {ex.Message}");
            Console.ResetColor();
            return false;
        }
    }

    static async Task<bool> Test3_LongResponseWithIdleTimeout(string apiKey)
    {
        try
        {
            var api = new OpenRouterModelApi(apiKey, "google/gemini-2.5-flash");
            
            // 30 секунд idle timeout для длинного ответа
            api.IdleTimeoutSettings = IdleTimeoutSettings.FromSeconds(30);

            Console.WriteLine("Модель: google/gemini-2.5-flash");
            Console.WriteLine("Idle timeout: 30 секунд");
            Console.WriteLine("Запрос: Напиши список из 10 интересных фактов о космосе");
            Console.WriteLine();

            var messages = new[]
            {
                new LLMMessage("user", "Напиши список из 10 интересных фактов о космосе")
            };

            var settings = new GenerateSettings(
                temperature: 0.7,
                maxTokens: 500
            );

            Console.Write("Отправка запроса (ожидается длинный ответ)... ");
            var startTime = DateTime.Now;
            var response = await api.SendWithContextAsync(messages, settings, CancellationToken.None);
            var elapsed = (DateTime.Now - startTime).TotalSeconds;
            
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"✓ Ответ получен за {elapsed:F1} секунд");
            Console.ResetColor();
            Console.WriteLine();
            
            var content = response.Choices.First().Message.Content.ToString();
            var contentPreview = content.Length > 200 ? content.Substring(0, 200) + "..." : content;
            Console.WriteLine($"Ответ (первые 200 символов): {contentPreview}");
            Console.WriteLine($"Полная длина ответа: {content.Length} символов");
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("✅ ТЕСТ ПРОЙДЕН - Длинный ответ получен успешно с idle timeout контролем");
            Console.ResetColor();
            return true;
        }
        catch (TimeoutException ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"❌ ТЕСТ НЕ ПРОЙДЕН - Idle Timeout: {ex.Message}");
            Console.WriteLine("   Между токенами прошло > 30 секунд");
            Console.ResetColor();
            return false;
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"❌ ТЕСТ НЕ ПРОЙДЕН: {ex.Message}");
            Console.ResetColor();
            return false;
        }
    }

    static async Task<bool> Test4_DisabledIdleTimeout(string apiKey)
    {
        try
        {
            var api = new OpenRouterModelApi(apiKey, "google/gemini-2.5-flash");
            
            // Отключаем idle timeout
            api.IdleTimeoutSettings = IdleTimeoutSettings.Disabled;

            Console.WriteLine("Модель: google/gemini-2.5-flash");
            Console.WriteLine("Idle timeout: ОТКЛЮЧЕН");
            Console.WriteLine("Запрос: Привет!");
            Console.WriteLine();

            var messages = new[]
            {
                new LLMMessage("user", "Привет!")
            };

            var settings = new GenerateSettings(
                temperature: 0.7,
                maxTokens: 20
            );

            Console.Write("Отправка запроса... ");
            var response = await api.SendWithContextAsync(messages, settings, CancellationToken.None);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("✓ Получен ответ");
            Console.ResetColor();

            var content = response.Choices.First().Message.Content;
            Console.WriteLine($"Ответ: {content}");
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("✅ ТЕСТ ПРОЙДЕН - Отключение idle timeout работает");
            Console.ResetColor();
            return true;
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"❌ ТЕСТ НЕ ПРОЙДЕН: {ex.Message}");
            Console.ResetColor();
            return false;
        }
    }

    #endregion

    #region Summary

    static void PrintSummary(int testsRun, int testsPassed, int testsFailed)
    {
        Console.WriteLine("=".PadRight(80, '='));
        Console.WriteLine("ИТОГИ ТЕСТИРОВАНИЯ");
        Console.WriteLine("=".PadRight(80, '='));
        Console.WriteLine($"Всего тестов:  {testsRun}");
        
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"Пройдено:      {testsPassed}");
        Console.ResetColor();
        
        if (testsFailed > 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Провалено:     {testsFailed}");
            Console.ResetColor();
        }
        else
        {
            Console.WriteLine($"Провалено:     {testsFailed}");
        }
        
        Console.WriteLine();
        
        if (testsFailed == 0)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("🎉 ВСЕ ТЕСТЫ ПРОШЛИ УСПЕШНО!");
            Console.WriteLine();
            Console.WriteLine("ВЫВОД: SendWithContextAsync ВСЕГДА использует streaming внутри!");
            Console.WriteLine("       Idle Timeout автоматически проверяет время между токенами.");
            Console.WriteLine("       Если между токенами > N секунд → TimeoutException.");
            Console.ResetColor();
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("❌ НЕКОТОРЫЕ ТЕСТЫ НЕ ПРОШЛИ!");
            Console.ResetColor();
        }
    }

    #endregion
}
