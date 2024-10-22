namespace LargeFileSorter.Models;

public record Row(int Number, string Value, string FileName = ""): IComparable<Row>
{
    public static Row? FromString(string text, string fileName = default)
    {
        var split = text.Split('.');
        return int.TryParse(split[0], out var number) 
            ? new Row(number, split[1], fileName) 
            : null;
    }
    
    public int CompareTo(Row? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        
        if (other is null) return 1;
        
        var stringComparison = string.Compare(Value, other.Value, StringComparison.Ordinal);
        return stringComparison != 0 
            ? stringComparison 
            : Number.CompareTo(other.Number);
    }
    
    public override int GetHashCode() => HashCode.Combine(Number, Value);
    
    public override string ToString() => $"{Number}.{Value}";
    
}