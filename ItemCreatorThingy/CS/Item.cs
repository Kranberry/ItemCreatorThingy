namespace ItemCreatorThingy.CS;

public class Item
{
    public string ItemClass { get; set; }
    public string Identifier { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Texture { get; set; }
    public int BuyPrice { get; set; }
    public int SellPrice { get; set; }
    public Dictionary<string, int> ResourceCost { get; set; }
    public Dictionary<string, int> DisassembleReturn { get; set; }
    public int CraftTime { get; set; }

    public static string GenerateNextId(List<Item> allItems, string itemClass)
    {
        IList<string> itemsIdentifiersInClass = allItems.Where(i => i.ItemClass == itemClass).Select(i => i.Identifier).ToList();
        IList<int> idNumbersInClass = itemsIdentifiersInClass.Select(str => int.Parse(str.Substring(3))).ToList();
        int highestIdNumber = idNumbersInClass.Count > 0 ? idNumbersInClass.Max() : 0;

        string firstChars =  itemClass[..2].ToLower();
        string fullNewId = $"{firstChars}-{highestIdNumber + 1}";

        return fullNewId;
    }
}