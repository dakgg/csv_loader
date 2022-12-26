using Newtonsoft.Json.Linq;

var dirInfo = new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), "csv"));
var csvFiles = dirInfo.GetFiles().Where(fi => fi.Extension == ".csv");
Dictionary<string, string> csvDic = new Dictionary<string, string>();

foreach (var fi in dirInfo.GetFiles().Where(fi => fi.Extension == ".csv"))
{
    csvDic.Add(Path.GetFileNameWithoutExtension(fi.Name), File.ReadAllText(fi.FullName));
    var jarr = ProcessCsv(File.ReadAllText(fi.FullName));

}





static JArray ProcessCsv(string csv)
{
    var jArray = new JArray();
    var strArr = csv.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
    var headers = ProcessCsvRow(strArr[0]).ToArray();
    for (int i = 1; i < strArr.Length; i++)
    {
        var newObj = new JObject();
        var values = ProcessCsvRow(strArr[i]).ToArray();
        for (int j = 0; j < headers.Length; j++)
        {
            if (string.IsNullOrWhiteSpace(values[j]) || values[j].Equals("#")) continue;
            newObj[headers[j]] = values[j];
        }
        jArray.Add(newObj);
    }
    return jArray;
}

static IEnumerable<string> ProcessCsvRow(string row)
{
    var index = 0;
    var startIndex = 0;

    var inquote = false;
    var hadquote = false;

    while (index <= row.Length)
    {
        var currentChar = row.Length > index ? row[index] : '\0';

        if ((!inquote && currentChar == ',') || currentChar == '\0')
        {
            var value = row.Substring(startIndex, index - startIndex).Trim();
            if (hadquote)
            {
                var trimmed = value.Replace("\"\"", "\"");
                value = trimmed.Substring(1, trimmed.Length - 2);
                hadquote = false;
            }
            startIndex = index + 1;
            //if string, recover line ending
            value = value.Replace("\\n", "\n");
            yield return value;
        }
        else if (currentChar == '\"')
        {
            if (row.Length <= index + 1 || row[index + 1] != '\"')
            {
                //we need to find next ending
                inquote = !inquote;
                hadquote = true;
            }
            else
            {
                //advance index as its two double quoted
                index++;
            }
        }
        index++;
    }
}