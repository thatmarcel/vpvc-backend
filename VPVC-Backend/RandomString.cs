namespace VPVC_Backend; 

// Class providing a simple method to generate random strings
public static class RandomString {
    private static readonly Random random = new();
    
    private const string characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    private const string uppercaseCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    public static string Generate(int length = 16) {
        return new string(Enumerable
            .Repeat(characters, length)
            .Select(s => s[random.Next(s.Length)])
            .ToArray()
        );
    }
    
    public static string GenerateUsingOnlyUppercaseLetters(int length = 16) {
        return new string(Enumerable
            .Repeat(uppercaseCharacters, length)
            .Select(s => s[random.Next(s.Length)])
            .ToArray()
        );
    }
}