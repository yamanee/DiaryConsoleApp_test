using CsvHelper.Configuration;
using CsvHelper;
using System.Globalization;

class Program
{
    static void Main(string[] args)
    {
        var diaryEntries = new List<DiaryEntry>();
        string csvFilePath = "diary.csv"; 

        if (args.Length > 0)
        {
            csvFilePath = args[0];
        }

        diaryEntries = LoadDiaryEntriesFromCsv(csvFilePath);

        while (true)
        {
            Console.WriteLine("日記アプリケーション");
            Console.WriteLine("1. 新しい日記を追加");
            Console.WriteLine("2. 日記を表示");
            Console.WriteLine("3. 日記を編集");
            Console.WriteLine("4. 日記を削除");
            Console.WriteLine("5. 日付検索");
            Console.WriteLine("6. 内容検索");
            Console.WriteLine("7. 終了");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    //新しい日記を追加
                    // すべてのエントリーの中で最大のIDを見つける
                    int maxId = diaryEntries.Count > 0 ? diaryEntries.Max(entry => entry.Id) : 0;
                    // 新しいエントリーに一意のIDを割り当てる
                    int newId = maxId + 1;
                    var entry = CreateDiaryEntry(newId);
                    if (entry != null)
                    {
                        diaryEntries.Add(entry);
                        Console.WriteLine("日記が追加されました。");
                    }
                    break;
                case "2":
                    //日記を表示
                    diaryEntries.Sort((entry1, entry2) => entry1.Date.CompareTo(entry2.Date));
                    Console.WriteLine("日記一覧:");
                    foreach (var entryitem in diaryEntries)
                    {
                        Console.WriteLine($"（{entryitem.Id}）日付: {entryitem.Date.ToShortDateString()}");
                        Console.WriteLine(entryitem.Content);
                        Console.WriteLine();
                    }
                    break;
                case "3":
                    // 日記を編集
                    Console.WriteLine("編集する日記のIDを入力してください:");
                    if (int.TryParse(Console.ReadLine(), out int editEntryId))
                    {
                        EditDiaryEntry(diaryEntries, editEntryId);
                    }
                    else
                    {
                        Console.WriteLine("無効なIDが入力されました。");
                    }
                    break;
                case "4":
                    // 日記を削除
                    Console.WriteLine("削除する日記のIDを入力してください:");
                    if (int.TryParse(Console.ReadLine(), out int entryId))
                    {
                        DeleteDiaryEntry(diaryEntries, entryId);
                    }
                    else
                    {
                        Console.WriteLine("無効なIDが入力されました。");
                    }
                    break;
                case "5":
                    //日付の範囲検索
                    SearchByDateRange(diaryEntries);
                    break;
                case "6":
                    //内容で検索
                    SearchByText(diaryEntries);
                    break;
                case "7":
                    Console.WriteLine("アプリケーションを終了します。");
                    //アプリケーションの終了前にcsvファイルを保存
                    SaveDiaryEntriesToCsv(csvFilePath, diaryEntries);
                    return;
                default:
                    Console.WriteLine("無効な選択です。もう一度お試しください。");
                    break;
            }
        }
    }

    //日記追加処理
    public static DiaryEntry CreateDiaryEntry(int newId)
    {
        Console.WriteLine("日付を入力してください (yyyy/MM/dd):");
        if (!DateTime.TryParse(Console.ReadLine(), out DateTime date))
        {
            Console.WriteLine("無効な日付形式です。");
            return null;
        }

        Console.WriteLine("日記の内容を入力してください:");
        string content = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(content))
        {
            Console.WriteLine("日記の内容が無効です。日記は登録されません。");
            return null;
        }

        return new DiaryEntry { Id = newId, Date = date, Content = content };
    }

    //日記の編集
    public static void EditDiaryEntry(List<DiaryEntry> entries, int entryId)
    {
        var entryToEdit = entries.FirstOrDefault(entry => entry.Id == entryId);

        if (entryToEdit != null)
        {
            Console.WriteLine($"現在の内容: {entryToEdit.Content}");
            Console.WriteLine("新しい内容を入力してください:");
            string newContent = Console.ReadLine();

            entryToEdit.Content = newContent;
            Console.WriteLine($"日記 ID {entryId} が編集されました。");
        }
        else
        {
            Console.WriteLine("指定したIDの日記が見つかりませんでした。");
        }
    }

    //日記の削除処理
    public static void DeleteDiaryEntry(List<DiaryEntry> entries, int entryId)
    {
        var entryToDelete = entries.FirstOrDefault(entry => entry.Id == entryId);

        if (entryToDelete != null)
        {
            entries.Remove(entryToDelete);
            Console.WriteLine($"日記 ID {entryId} が削除されました。");
        }
        else
        {
            Console.WriteLine("指定したIDの日記が見つかりませんでした。");
        }
    }


    //日記の日付検索処理
    public static void SearchByDateRange(List<DiaryEntry> entries)
    {
        Console.WriteLine("検索開始日を入力してください (yyyy/MM/dd):");
        if (!DateTime.TryParse(Console.ReadLine(), out DateTime startDate))
        {
            Console.WriteLine("無効な日付形式です。");
            return;
        }

        Console.WriteLine("検索終了日を入力してください (yyyy/MM/dd):");
        if (!DateTime.TryParse(Console.ReadLine(), out DateTime endDate))
        {
            Console.WriteLine("無効な日付形式です。");
            return;
        }

        var matchingEntries = entries.FindAll(entry => entry.Date.Date >= startDate.Date && entry.Date.Date <= endDate.Date);

        if (matchingEntries.Count == 0)
        {
            Console.WriteLine("指定した日付範囲内の日記は見つかりませんでした。");
        }
        else
        {
            Console.WriteLine($"日記（{startDate.ToShortDateString()}-{endDate.ToShortDateString()}）:");
            foreach (var entry in matchingEntries)
            {
                Console.WriteLine($"日付: {entry.Date.ToShortDateString()}");
                Console.WriteLine(entry.Content);
                Console.WriteLine();
            }
        }
    }

    //日記の内容検索処理
    public static void SearchByText(List<DiaryEntry> entries)
    {
        Console.WriteLine("検索するテキストを入力してください:");
        string searchText = Console.ReadLine();

        var matchingEntries = entries.FindAll(entry => entry.Content.Contains(searchText));

        if (matchingEntries.Count == 0)
        {
            Console.WriteLine("指定したテキストを含む日記は見つかりませんでした。");
        }
        else
        {
            Console.WriteLine($"日記({searchText}を含む):");
            foreach (var entry in matchingEntries)
            {
                Console.WriteLine($"日付: {entry.Date.ToShortDateString()}");
                HighlightAndPrintText(entry.Content, searchText);
                Console.WriteLine();
            }
        }
    }

    //検索した文字を赤くして出力する処理
    public static void HighlightAndPrintText(string text, string searchTerm)
    {
        int lastIndex = 0;

        while (true)
        {
            int index = text.IndexOf(searchTerm, lastIndex, StringComparison.OrdinalIgnoreCase);

            if (index == -1)
            {
                Console.Write(text.Substring(lastIndex));
                break;
            }

            Console.Write(text.Substring(lastIndex, index - lastIndex));

            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(text.Substring(index, searchTerm.Length));
            Console.ResetColor();

            lastIndex = index + searchTerm.Length;
        }

        Console.WriteLine();
    }

    //CSVファイル読み込み処理
    public static List<DiaryEntry> LoadDiaryEntriesFromCsv(string filePath)
    {
        if (File.Exists(filePath))
        {
            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)))
            {
                return csv.GetRecords<DiaryEntry>().ToList();
            }
        }
        else
        {
            return new List<DiaryEntry>();
        }
    }

    //CSVファイル保存処理
    public static void SaveDiaryEntriesToCsv(string filePath, List<DiaryEntry> entries)
    {
        using (var writer = new StreamWriter(filePath))
        using (var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)))
        {
            csv.WriteRecords(entries);
        }
    }


}

public class DiaryEntry
{
    public int Id { get; set; } // 一意のID
    //日付
    public DateTime Date { get; set; }
    //内容
    public string Content { get; set; }
}