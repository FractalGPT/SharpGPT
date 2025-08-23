using System.Text.RegularExpressions;

string ClearStr(string str)
{
    if (string.IsNullOrEmpty(str))
        return string.Empty;

    // Удаляем все символы кроме английских/русских букв и цифр с начала и конца
    return Regex.Replace(str, @"^[^a-zA-Zа-яА-ЯёЁ0-9]+|[^a-zA-Zа-яА-ЯёЁ0-9]+$", "").ToLower();
}

List<KeyValuePair<string, string>> list = [];
list.Add(KeyValuePair.Create("...Hello, World!!!", "hello, world"));
list.Add(KeyValuePair.Create("---Привет123---", "привет123"));
list.Add(KeyValuePair.Create("!!!Test??", "test"));
list.Add(KeyValuePair.Create("   Текст   ", "текст"));

foreach (var item in list)
{
    var result = ClearStr(item.Key);
    Console.WriteLine($"Вход={item.Key}\n#####\nОжидаемый выход={item.Value}\n#####\nРезультат={result}\n#####\nПроверка={result == item.Value}\n");
}